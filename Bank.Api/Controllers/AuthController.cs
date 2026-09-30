using Bank.Api.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var response = await _service.LoginAsync(dto);
        return response is null ? Unauthorized(new { message = "Email ou mot de passe invalide." }) : Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("agents")]
    public async Task<ActionResult<UserDto>> CreateAgent(CreateAgentDto dto)
    {
        try
        {
            var user = await _service.CreateAgentAsync(dto);
            return Created($"api/auth/agents/{user.Id}", user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
