using Microsoft.Extensions.Logging;
using WMS.Application.Common;
using WMS.Application.DTOs.Client;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ILogger<ClientService> _logger;

        public ClientService(
            IClientRepository clientRepository,
            ILogger<ClientService> logger)
        {
            _clientRepository = clientRepository;
            _logger = logger;
        }

        public async Task<List<ClientDto>> GetAllClientsAsync(SearchRequestDto request)
        {
            var records = await _clientRepository.GetAllClientsAsync(request.SearchTerm);
            return records.Select(MapToDto).ToList();
        }

        public async Task<(bool Success, string Message, ClientDto? Data)> GetClientByIdAsync(int id)
        {
            var client = await _clientRepository.GetClientByIdAsync(id);
            if (client == null)
            {
                return (false, "Client not found.", null);
            }

            return (true, "Client found.", MapToDto(client));
        }

        public async Task<(bool Success, string Message, ClientDto? Data)> CreateClientAsync(CreateClientDto request, int userId)
        {
            var client = new Client
            {
                ClientName = request.ClientName,
                ClientAdress = request.ClientAdress,
                ClientPhoneNumber = request.ClientPhoneNumber,
                ClientLocation = request.ClientLocation,
                Status = request.Status
            };

            var created = await _clientRepository.AddClientAsync(client, userId);
            
            _logger.LogInformation("Client created: {ClientId} by user {UserId}", created.ClientId, userId);

            return (true, "Client created successfully.", MapToDto(created));
        }

        public async Task<(bool Success, string Message, ClientDto? Data)> UpdateClientAsync(int id, UpdateClientDto request, int userId)
        {
            var client = await _clientRepository.GetClientByIdAsync(id);
            if (client == null)
            {
                return (false, "Client not found.", null);
            }

            client.ClientName = request.ClientName;
            client.ClientAdress = request.ClientAdress;
            client.ClientPhoneNumber = request.ClientPhoneNumber;
            client.ClientLocation = request.ClientLocation;
            client.Status = request.Status;

            await _clientRepository.UpdateClientAsync(client, userId);

            _logger.LogInformation("Client updated: {ClientId} by user {UserId}", client.ClientId, userId);

            return (true, "Client updated successfully.", MapToDto(client));
        }

        public async Task<(bool Success, string Message)> DeleteClientAsync(int id, int userId)
        {
            var client = await _clientRepository.GetClientByIdAsync(id);
            if (client == null)
            {
                return (false, "Client not found.");
            }

            // Optional check: if client has projects, we might reject delete.
            // But we don't have GetProjectsByClient in IProjectRepository immediately available.
            // We'll let SQL handle foreign key conflicts if any exist.

            await _clientRepository.DeleteClientAsync(client, userId);

            _logger.LogInformation("Client deleted: {ClientId} by user {UserId}", id, userId);

            return (true, "Client deleted successfully.");
        }

        private ClientDto MapToDto(Client client)
        {
            return new ClientDto
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                ClientAdress = client.ClientAdress,
                ClientPhoneNumber = client.ClientPhoneNumber,
                ClientLocation = client.ClientLocation,
                Status = client.Status
            };
        }
    }
}
