using Bank.Api.Data;
using Bank.Api.DTOs.TypesComptes;
using Bank.Api.Models;
using Bank.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Services.Implementations;

public class TypeCompteService : ITypeCompteService
{
    private readonly AppDbContext _context;

    public TypeCompteService(AppDbContext context) => _context = context;

    public Task<List<TypeCompteDto>> GetAllAsync() => _context.TypesComptes.AsNoTracking()
        .OrderBy(t => t.Code).Select(t => ToDto(t)).ToListAsync();

    public async Task<TypeCompteDto?> GetByIdAsync(int id)
    {
        var type = await _context.TypesComptes.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);
        return type is null ? null : ToDto(type);
    }

    public async Task<TypeCompteDto> CreateAsync(CreateTypeCompteDto dto)
    {
        if (await _context.TypesComptes.AnyAsync(t => t.Code == dto.Code))
            throw new InvalidOperationException("Un type de compte avec ce code existe déjà.");
        var type = new TypeCompte { Code = dto.Code, Libelle = dto.Libelle, Description = dto.Description, SoldeMinimum = dto.SoldeMinimum, FraisMensuels = dto.FraisMensuels };
        _context.TypesComptes.Add(type);
        await _context.SaveChangesAsync();
        return ToDto(type);
    }

    public async Task<bool> UpdateAsync(int id, UpdateTypeCompteDto dto)
    {
        var type = await _context.TypesComptes.FindAsync(id);
        if (type is null) return false;
        type.Libelle = dto.Libelle;
        type.Description = dto.Description;
        type.SoldeMinimum = dto.SoldeMinimum;
        type.FraisMensuels = dto.FraisMensuels;
        type.IsActive = dto.IsActive;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var type = await _context.TypesComptes.FindAsync(id);
        if (type is null) return false;
        type.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private static TypeCompteDto ToDto(TypeCompte t) => new()
    {
        Id = t.Id, Code = t.Code, Libelle = t.Libelle, Description = t.Description,
        SoldeMinimum = t.SoldeMinimum, FraisMensuels = t.FraisMensuels, IsActive = t.IsActive
    };
}
