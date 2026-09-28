using Bank.Api.Data;
using Bank.Api.DTOs.Transactions;
using Bank.Api.Enums;
using Bank.Api.Models;
using Bank.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Services.Implementations;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;

    public TransactionService(AppDbContext context) => _context = context;

    public Task<List<TransactionDto>> GetByCompteAsync(int compteId) => _context.Transactions.AsNoTracking()
        .Where(t => t.CompteSourceId == compteId || t.CompteDestinationId == compteId)
        .OrderByDescending(t => t.DateOperation).Select(t => ToDto(t)).ToListAsync();

    public Task<TransactionDto> DepotAsync(DepotDto dto, string agentId) =>
        ExecuteAsync(TypeTransaction.Depot, dto.CompteId, null, dto.Montant, dto.Description, agentId);

    public Task<TransactionDto> RetraitAsync(RetraitDto dto, string agentId) =>
        ExecuteAsync(TypeTransaction.Retrait, dto.CompteId, null, dto.Montant, dto.Description, agentId);

    public Task<TransactionDto> VirementAsync(VirementDto dto, string agentId)
    {
        if (dto.CompteSourceId == dto.CompteDestinationId)
            throw new InvalidOperationException("Les comptes source et destination doivent être différents.");
        return ExecuteAsync(TypeTransaction.Virement, dto.CompteSourceId, dto.CompteDestinationId, dto.Montant, dto.Description, agentId);
    }

    private async Task<TransactionDto> ExecuteAsync(TypeTransaction type, int sourceId, int? destinationId, decimal montant, string? description, string agentId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == agentId && u.IsActive))
            throw new InvalidOperationException("L'agent est introuvable ou inactif.");

        await using var dbTransaction = await _context.Database.BeginTransactionAsync();
        var source = await _context.Comptes.Include(c => c.TypeCompte)
            .SingleOrDefaultAsync(c => c.Id == sourceId && c.IsActive)
            ?? throw new KeyNotFoundException("Le compte source est introuvable ou inactif.");

        Compte? destination = null;
        if (destinationId.HasValue)
            destination = await _context.Comptes.SingleOrDefaultAsync(c => c.Id == destinationId && c.IsActive)
                ?? throw new KeyNotFoundException("Le compte destination est introuvable ou inactif.");

        if (type == TypeTransaction.Depot)
            source.Solde += montant;
        else
        {
            if (source.Solde - montant < source.TypeCompte.SoldeMinimum)
                throw new InvalidOperationException("Solde insuffisant pour respecter le solde minimum.");
            source.Solde -= montant;
            if (destination is not null) destination.Solde += montant;
        }

        var operation = new Transaction
        {
            Reference = $"TRX-{Guid.NewGuid():N}".ToUpperInvariant(), Type = type, Montant = montant,
            Description = description, CompteSourceId = sourceId, CompteDestinationId = destinationId, AgentId = agentId
        };
        _context.Transactions.Add(operation);
        await _context.SaveChangesAsync();
        await dbTransaction.CommitAsync();
        return ToDto(operation);
    }

    private static TransactionDto ToDto(Transaction t) => new()
    {
        Id = t.Id, Reference = t.Reference, Type = t.Type, Montant = t.Montant,
        DateOperation = t.DateOperation, Description = t.Description, CompteSourceId = t.CompteSourceId,
        CompteDestinationId = t.CompteDestinationId, AgentId = t.AgentId
    };
}
