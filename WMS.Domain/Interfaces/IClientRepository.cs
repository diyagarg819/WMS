using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    public interface IClientRepository
    {
        Task<List<Client>> GetAllClientsAsync(string? searchTerm);
        Task<Client?> GetClientByIdAsync(int id);
        Task<Client> AddClientAsync(Client client, int createdByUserId);
        Task UpdateClientAsync(Client client, int updatedByUserId);
        Task DeleteClientAsync(Client client, int deletedByUserId);
    }
}
