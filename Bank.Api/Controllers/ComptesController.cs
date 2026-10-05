using Bank.Api.DTOs.Comptes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ComptesController : ControllerBase
{
    private readonly ICompteService _service;

    public ComptesController(ICompteService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<CompteDto>>> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CompteDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<CompteDto>> Create(CreateCompteDto dto)
    {
        try
        {
            var item = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateCompteDto dto) =>
        await _service.UpdateAsync(id, dto) ? NoContent() : NotFound();
}
