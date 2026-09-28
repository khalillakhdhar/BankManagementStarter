using Bank.Api.DTOs.Transactions;

namespace Bank.Api.Services.Interfaces;

public interface ITransactionService
{
    Task<List<TransactionDto>> GetByCompteAsync(int compteId);
    Task<TransactionDto> DepotAsync(DepotDto dto, string agentId);
    Task<TransactionDto> RetraitAsync(RetraitDto dto, string agentId);
    Task<TransactionDto> VirementAsync(VirementDto dto, string agentId);
}
