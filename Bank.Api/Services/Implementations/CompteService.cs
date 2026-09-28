using Bank.Api.Data;
using Bank.Api.DTOs.Comptes;
using Bank.Api.Models;
using Bank.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Services.Implementations;

public class CompteService : ICompteService
{
    private readonly AppDbContext _context;

    public CompteService(AppDbContext context) => _context = context;

    public Task<List<CompteDto>> GetAllAsync() => Query().OrderBy(c => c.NumeroCompte)
        .Select(c => ToDto(c)).ToListAsync();

    public async Task<CompteDto?> GetByIdAsync(int id)
    {
        var compte = await Query().SingleOrDefaultAsync(c => c.Id == id);
        return compte is null ? null : ToDto(compte);
    }

    public async Task<CompteDto> CreateAsync(CreateCompteDto dto)
    {
        if (await _context.Comptes.AnyAsync(c => c.NumeroCompte == dto.NumeroCompte))
            throw new InvalidOperationException("Un compte avec ce numéro existe déjà.");

        var clientExists = await _context.Clients.AnyAsync(c => c.Id == dto.ClientId && c.IsActive);
        var type = await _context.TypesComptes.SingleOrDefaultAsync(t => t.Id == dto.TypeCompteId && t.IsActive);
        var guichetExists = await _context.Guichets.AnyAsync(g => g.Id == dto.GuichetId && g.IsActive);
        if (!clientExists || type is null || !guichetExists)
            throw new InvalidOperationException("Le client, le type de compte ou le guichet est introuvable ou inactif.");
        if (dto.SoldeInitial < type.SoldeMinimum)
            throw new InvalidOperationException($"Le solde initial minimum est {type.SoldeMinimum:0.00}.");

        var compte = new Compte
        {
            NumeroCompte = dto.NumeroCompte, Solde = dto.SoldeInitial, ClientId = dto.ClientId,
            TypeCompteId = dto.TypeCompteId, GuichetId = dto.GuichetId
        };
        _context.Comptes.Add(compte);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(compte.Id))!;
    }

    public async Task<bool> UpdateAsync(int id, UpdateCompteDto dto)
    {
        var compte = await _context.Comptes.FindAsync(id);
        if (compte is null) return false;
        compte.IsActive = dto.IsActive;
        await _context.SaveChangesAsync();
        return true;
    }

    private IQueryable<Compte> Query() => _context.Comptes.AsNoTracking()
        .Include(c => c.Client).Include(c => c.TypeCompte).Include(c => c.Guichet);

    private static CompteDto ToDto(Compte c) => new()
    {
        Id = c.Id, NumeroCompte = c.NumeroCompte, Solde = c.Solde, DateOuverture = c.DateOuverture,
        IsActive = c.IsActive, ClientId = c.ClientId, ClientNomComplet = $"{c.Client.Prenom} {c.Client.Nom}",
        TypeCompteId = c.TypeCompteId, TypeCompte = c.TypeCompte.Libelle,
        GuichetId = c.GuichetId, Guichet = c.Guichet.Nom
    };
}
