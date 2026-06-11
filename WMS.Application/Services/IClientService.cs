using WMS.Application.Common;
using WMS.Application.DTOs.Client;

namespace WMS.Application.Services
{
    public interface IClientService
    {
        Task<List<ClientDto>> GetAllClientsAsync(SearchRequestDto request);
        Task<(bool Success, string Message, ClientDto? Data)> GetClientByIdAsync(int id);
        Task<(bool Success, string Message, ClientDto? Data)> CreateClientAsync(CreateClientDto request, int userId);
        Task<(bool Success, string Message, ClientDto? Data)> UpdateClientAsync(int id, UpdateClientDto request, int userId);
        Task<(bool Success, string Message)> DeleteClientAsync(int id, int userId);
    }
}
