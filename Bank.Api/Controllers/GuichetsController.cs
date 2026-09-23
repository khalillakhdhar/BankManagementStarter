using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GuichetsController : ControllerBase
{
    private readonly IGuichetService _service;

    public GuichetsController(IGuichetService service)
    {
        _service = service;
    }

    // TODO :
    // GET / POST / PUT / DELETE logique
}
