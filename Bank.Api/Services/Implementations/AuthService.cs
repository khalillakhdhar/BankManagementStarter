using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bank.Api.Configuration;
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

    public AuthService(UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
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
        if (dto.GuichetId.HasValue &&
            !await _context.Guichets.AnyAsync(g => g.Id == dto.GuichetId && g.IsActive))
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
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(error => error.Description)));

        await _userManager.AddToRoleAsync(user, "Agent");
        return await ToDtoAsync(user);
    }

    private async Task<AuthResponseDto> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var expiration = DateTime.UtcNow.AddMinutes(LocalDevelopmentSettings.JwtExpirationMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: LocalDevelopmentSettings.JwtIssuer,
            audience: LocalDevelopmentSettings.JwtAudience,
            claims: claims,
            expires: expiration,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(LocalDevelopmentSettings.JwtSecret)),
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
