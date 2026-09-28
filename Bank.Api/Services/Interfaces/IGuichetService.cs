using Bank.Api.DTOs.Guichets;

namespace Bank.Api.Services.Interfaces;

public interface IGuichetService
{
    Task<List<GuichetDto>> GetAllAsync();
    Task<GuichetDto?> GetByIdAsync(int id);
    Task<GuichetDto> CreateAsync(CreateGuichetDto dto);
    Task<bool> UpdateAsync(int id, UpdateGuichetDto dto);
    Task<bool> DeleteAsync(int id);
}
