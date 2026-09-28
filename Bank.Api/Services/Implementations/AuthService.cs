using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bank.Api.Data;
using Bank.Api.Models;
using Bank.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Bank.Api.DTOs.Auth;

namespace Bank.Api.Services.Implementations;


public class AuthService : IAuthService
{
    // TODO SÉANCE SUIVANTE :
    // Injecter AppDbContext et implémenter les méthodes métier.
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext context;
    private IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> _userManager,
         AppDbContext context,
         IConfiguration _configuration
        )
    {
        this._userManager = _userManager;
        this.context = context;
        this._configuration = _configuration;
    }
    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user= await _userManager
            .FindByEmailAsync(dto.Email);
        if (user == null) {
            return null;
        }
        if(!user.IsActive)
        {
            return null;
        }
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
            return null;
        else         {
            var token = GenerateJwtToken(user);
            return token;
        }

    }
    private AuthResponseDto GenerateJwtToken(ApplicationUser user)
    {
        return null;
    }
    private static UserDto toDto(ApplicationUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Nom = user.Nom,
            Prenom = user.Prenom,
            IsActive = user.IsActive
        };
    }

}
