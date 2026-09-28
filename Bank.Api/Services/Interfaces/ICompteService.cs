using Bank.Api.DTOs.Comptes;

namespace Bank.Api.Services.Interfaces;

public interface ICompteService
{
    Task<List<CompteDto>> GetAllAsync();
    Task<CompteDto?> GetByIdAsync(int id);
    Task<CompteDto> CreateAsync(CreateCompteDto dto);
    Task<bool> UpdateAsync(int id, UpdateCompteDto dto);
}
