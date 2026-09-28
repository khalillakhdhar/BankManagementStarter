using Bank.Api.DTOs.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _service;

    public TransactionsController(ITransactionService service)
    {
        _service = service;
    }

    [HttpGet("compte/{compteId:int}")]
    public async Task<ActionResult<List<TransactionDto>>> GetByCompte(int compteId) =>
        Ok(await _service.GetByCompteAsync(compteId));

    [HttpPost("depot")]
    public Task<ActionResult<TransactionDto>> Depot(DepotDto dto, [FromQuery] string agentId) =>
        Execute(() => _service.DepotAsync(dto, agentId));

    [HttpPost("retrait")]
    public Task<ActionResult<TransactionDto>> Retrait(RetraitDto dto, [FromQuery] string agentId) =>
        Execute(() => _service.RetraitAsync(dto, agentId));

    [HttpPost("virement")]
    public Task<ActionResult<TransactionDto>> Virement(VirementDto dto, [FromQuery] string agentId) =>
        Execute(() => _service.VirementAsync(dto, agentId));

    private async Task<ActionResult<TransactionDto>> Execute(Func<Task<TransactionDto>> action)
    {
        try { return Ok(await action()); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
