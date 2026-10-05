using Bank.Api.DTOs.Auth;

namespace Bank.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto dto) => throw new NotImplementedException();
    Task<UserDto> CreateAgentAsync(CreateAgentDto dto) => throw new NotImplementedException();
}
