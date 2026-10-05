using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // TODO TP SÉCURITÉ :
    // - injecter IAuthService ;
    // - implémenter POST api/auth/login ;
    // - implémenter POST api/auth/agents ;
    // - ajouter l'authentification JWT et l'autorisation par rôles.
}
