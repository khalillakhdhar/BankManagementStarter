using Bank.Api.DTOs.TypesComptes;

namespace Bank.Api.Services.Interfaces;

public interface ITypeCompteService
{
    Task<List<TypeCompteDto>> GetAllAsync() => throw new NotImplementedException();
    Task<TypeCompteDto?> GetByIdAsync(int id) => throw new NotImplementedException();
    Task<TypeCompteDto> CreateAsync(CreateTypeCompteDto dto) => throw new NotImplementedException();
    Task<bool> UpdateAsync(int id, UpdateTypeCompteDto dto) => throw new NotImplementedException();
    Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
}
