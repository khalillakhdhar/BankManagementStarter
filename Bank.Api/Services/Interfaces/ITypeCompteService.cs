using Bank.Api.DTOs.TypesComptes;

namespace Bank.Api.Services.Interfaces;

public interface ITypeCompteService
{
    Task<List<TypeCompteDto>> GetAllAsync();
    Task<TypeCompteDto?> GetByIdAsync(int id);
    Task<TypeCompteDto> CreateAsync(CreateTypeCompteDto dto);
    Task<bool> UpdateAsync(int id, UpdateTypeCompteDto dto);
    Task<bool> DeleteAsync(int id);
}
