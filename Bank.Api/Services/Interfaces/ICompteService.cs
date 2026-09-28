using Bank.Api.DTOs.Comptes;

namespace Bank.Api.Services.Interfaces;

public interface ICompteService
{
    Task<List<CompteDto>> GetAllAsync() => throw new NotImplementedException();
    Task<CompteDto?> GetByIdAsync(int id) => throw new NotImplementedException();
    Task<CompteDto> CreateAsync(CreateCompteDto dto) => throw new NotImplementedException();
    Task<bool> UpdateAsync(int id, UpdateCompteDto dto) => throw new NotImplementedException();
}
