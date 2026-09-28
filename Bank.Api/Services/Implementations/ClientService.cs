using Bank.Api.Data;
using Bank.Api.DTOs.Clients;
using Bank.Api.DTOs.Comptes;
using Bank.Api.Models;
using Bank.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Services.Implementations;

public class ClientService : IClientService
{
    private readonly AppDbContext _context;

    public ClientService(AppDbContext context) => _context = context;

    public Task<List<ClientDto>> GetAllAsync() => _context.Clients
        .AsNoTracking()
        .OrderBy(c => c.Nom).ThenBy(c => c.Prenom)
        .Select(c => ToDto(c))
        .ToListAsync();

    public async Task<ClientDetailsDto?> GetByIdAsync(int id)
    {
        var client = await _context.Clients
            .AsNoTracking()
            .Include(c => c.Comptes)
            .ThenInclude(c => c.TypeCompte)
            .SingleOrDefaultAsync(c => c.Id == id);

        if (client is null) return null;

        return new ClientDetailsDto
        {
            Id = client.Id,
            Cin = client.Cin,
            Nom = client.Nom,
            Prenom = client.Prenom,
            DateNaissance = client.DateNaissance,
            Email = client.Email,
            Telephone = client.Telephone,
            Adresse = client.Adresse,
            DateCreation = client.DateCreation,
            IsActive = client.IsActive,
            Comptes = client.Comptes.Select(c => new CompteSummaryDto
            {
                Id = c.Id,
                NumeroCompte = c.NumeroCompte,
                Solde = c.Solde,
                TypeCompte = c.TypeCompte.Libelle,
                IsActive = c.IsActive
            }).ToList()
        };
    }

    public Task<List<ClientDto>> SearchAsync(string term)
    {
        term = term.Trim();
        return _context.Clients.AsNoTracking()
            .Where(c => c.Cin.Contains(term) || c.Nom.Contains(term) || c.Prenom.Contains(term))
            .OrderBy(c => c.Nom).ThenBy(c => c.Prenom)
            .Select(c => ToDto(c))
            .ToListAsync();
    }

    public async Task<ClientDto> CreateAsync(CreateClientDto dto)
    {
        if (await _context.Clients.AnyAsync(c => c.Cin == dto.Cin))
            throw new InvalidOperationException("Un client avec ce CIN existe déjà.");

        var client = new Client
        {
            Cin = dto.Cin,
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            DateNaissance = dto.DateNaissance,
            Email = dto.Email,
            Telephone = dto.Telephone,
            Adresse = dto.Adresse
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return ToDto(client);
    }

    public async Task<bool> UpdateAsync(int id, UpdateClientDto dto)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client is null) return false;

        client.Nom = dto.Nom;
        client.Prenom = dto.Prenom;
        client.DateNaissance = dto.DateNaissance;
        client.Email = dto.Email;
        client.Telephone = dto.Telephone;
        client.Adresse = dto.Adresse;
        client.IsActive = dto.IsActive;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client is null) return false;

        client.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private static ClientDto ToDto(Client client) => new()
    {
        Id = client.Id,
        Cin = client.Cin,
        Nom = client.Nom,
        Prenom = client.Prenom,
        DateNaissance = client.DateNaissance,
        Email = client.Email,
        Telephone = client.Telephone,
        Adresse = client.Adresse,
        DateCreation = client.DateCreation,
        IsActive = client.IsActive
    };
}
