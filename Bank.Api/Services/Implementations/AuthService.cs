using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bank.Api.Data;
using Bank.Api.DTOs.Auth;
using Bank.Api.Models;
using Bank.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Bank.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager, AppDbContext context, IConfiguration configuration)
    {
        _userManager = userManager;
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null || !user.IsActive || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return null;

        return await GenerateJwtTokenAsync(user);
    }

    public async Task<UserDto> CreateAgentAsync(CreateAgentDto dto)
    {
        if (dto.GuichetId.HasValue && !await _context.Guichets.AnyAsync(g => g.Id == dto.GuichetId && g.IsActive))
            throw new InvalidOperationException("Le guichet est introuvable ou inactif.");

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            GuichetId = dto.GuichetId,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "Agent");
        return await ToDtoAsync(user);
    }

    private async Task<AuthResponseDto> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var expiration = DateTime.UtcNow.AddMinutes(_configuration.GetValue("Jwt:ExpirationMinutes", 120));
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var secret = _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("La clé Jwt:Secret est manquante.");
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                SecurityAlgorithms.HmacSha256));

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration,
            User = await ToDtoAsync(user)
        };
    }

    private async Task<UserDto> ToDtoAsync(ApplicationUser user) => new()
    {
        Id = user.Id,
        Email = user.Email ?? string.Empty,
        Nom = user.Nom,
        Prenom = user.Prenom,
        GuichetId = user.GuichetId,
        IsActive = user.IsActive,
        Roles = (await _userManager.GetRolesAsync(user)).ToList()
    };
}
