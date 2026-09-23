using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TypesComptesController : ControllerBase
{
    private readonly ITypeCompteService _service;

    public TypesComptesController(ITypeCompteService service)
    {
        _service = service;
    }

    // TODO :
    // CRUD types de comptes
}
