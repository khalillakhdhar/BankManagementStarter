using Bank.Api.DTOs.Transactions;

namespace Bank.Api.Services.Interfaces;

public interface ITransactionService
{
    Task<List<TransactionDto>> GetByCompteAsync(int compteId) => throw new NotImplementedException();
    Task<TransactionDto> DepotAsync(DepotDto dto, string agentId) => throw new NotImplementedException();
    Task<TransactionDto> RetraitAsync(RetraitDto dto, string agentId) => throw new NotImplementedException();
    Task<TransactionDto> VirementAsync(VirementDto dto, string agentId) => throw new NotImplementedException();
}
