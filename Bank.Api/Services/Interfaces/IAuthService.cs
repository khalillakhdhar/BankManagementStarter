using Bank.Api.DTOs.Auth;

namespace Bank.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<UserDto> CreateAgentAsync(CreateAgentDto dto);
}
