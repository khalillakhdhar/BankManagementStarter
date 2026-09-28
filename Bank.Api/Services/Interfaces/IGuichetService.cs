using Bank.Api.DTOs.Guichets;

namespace Bank.Api.Services.Interfaces;

public interface IGuichetService
{
    Task<List<GuichetDto>> GetAllAsync() => throw new NotImplementedException();
    Task<GuichetDto?> GetByIdAsync(int id) => throw new NotImplementedException();
    Task<GuichetDto> CreateAsync(CreateGuichetDto dto) => throw new NotImplementedException();
    Task<bool> UpdateAsync(int id, UpdateGuichetDto dto) => throw new NotImplementedException();
    Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
}
