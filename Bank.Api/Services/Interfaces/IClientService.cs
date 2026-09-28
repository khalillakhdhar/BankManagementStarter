using Bank.Api.DTOs.Clients;

namespace Bank.Api.Services.Interfaces;

public interface IClientService
{
    Task<List<ClientDto>> GetAllAsync();
    Task<ClientDetailsDto?> GetByIdAsync(int id);
    Task<List<ClientDto>> SearchAsync(string term);
    Task<ClientDto> CreateAsync(CreateClientDto dto);
    Task<bool> UpdateAsync(int id, UpdateClientDto dto);
    Task<bool> DeleteAsync(int id);
}
