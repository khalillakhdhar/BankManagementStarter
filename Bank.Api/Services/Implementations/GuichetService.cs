using Bank.Api.Data;
using Bank.Api.DTOs.Guichets;
using Bank.Api.Models;
using Bank.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Services.Implementations;

public class GuichetService : IGuichetService
{
    private readonly AppDbContext _context;

    public GuichetService(AppDbContext context) => _context = context;

    public Task<List<GuichetDto>> GetAllAsync() => _context.Guichets.AsNoTracking()
        .OrderBy(g => g.Code).Select(g => ToDto(g)).ToListAsync();

    public async Task<GuichetDto?> GetByIdAsync(int id)
    {
        var guichet = await _context.Guichets.AsNoTracking()
            .Include(g => g.Agents).SingleOrDefaultAsync(g => g.Id == id);
        return guichet is null ? null : ToDto(guichet);
    }

    public async Task<GuichetDto> CreateAsync(CreateGuichetDto dto)
    {
        if (await _context.Guichets.AnyAsync(g => g.Code == dto.Code))
            throw new InvalidOperationException("Un guichet avec ce code existe déjà.");

        var guichet = new Guichet { Code = dto.Code, Nom = dto.Nom, Adresse = dto.Adresse, Ville = dto.Ville, Telephone = dto.Telephone };
        _context.Guichets.Add(guichet);
        await _context.SaveChangesAsync();
        return ToDto(guichet);
    }

    public async Task<bool> UpdateAsync(int id, UpdateGuichetDto dto)
    {
        var guichet = await _context.Guichets.FindAsync(id);
        if (guichet is null) return false;
        guichet.Nom = dto.Nom;
        guichet.Adresse = dto.Adresse;
        guichet.Ville = dto.Ville;
        guichet.Telephone = dto.Telephone;
        guichet.IsActive = dto.IsActive;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var guichet = await _context.Guichets.FindAsync(id);
        if (guichet is null) return false;
        guichet.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private static GuichetDto ToDto(Guichet g) => new()
    {
        Id = g.Id, Code = g.Code, Nom = g.Nom, Adresse = g.Adresse, Ville = g.Ville,
        Telephone = g.Telephone, IsActive = g.IsActive, DateCreation = g.DateCreation,
        NombreAgents = g.Agents.Count
    };
}
