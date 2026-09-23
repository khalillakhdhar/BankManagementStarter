using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComptesController : ControllerBase
{
    private readonly ICompteService _service;

    public ComptesController(ICompteService service)
    {
        _service = service;
    }

    // TODO :
    // ouverture / lecture / activation
}
