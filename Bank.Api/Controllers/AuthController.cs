using Bank.Api.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    /*  public AuthController(IAuthService service) { 
      service = _service;
      }
    */

  
    public AuthController(IAuthService service) => _service = service;
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    { 
    var response= await _service.LoginAsync(dto);
    return response is null ? Unauthorized(new {message="Email ou mot de passe invalide" }) : Ok(response);


    }

    // create agent
    [Authorize(Roles = "Admin")]
    [HttpPost("agent")]
    public async Task<ActionResult<UserDto>> CreateAgent(CreateAgentDto dto)
    {
        try
        {
            var user = await _service.CreateAgentAsync(dto);
            return Created($"api/auth/{user.Id}", user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }

    }
}
