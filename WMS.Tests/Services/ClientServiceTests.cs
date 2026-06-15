using Microsoft.Extensions.Logging;
using Moq;
using WMS.Application.Common;
using WMS.Application.DTOs.Client;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services
{
    public class ClientServiceTests
    {
        private readonly Mock<IClientRepository> _mockRepository;
        private readonly Mock<ILogger<ClientService>> _mockLogger;
        private readonly ClientService _clientService;

        public ClientServiceTests()
        {
            _mockRepository = new Mock<IClientRepository>();
            _mockLogger = new Mock<ILogger<ClientService>>();
            _clientService = new ClientService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllClients_ReturnsList()
        {
            var clients = new List<Client>
            {
                new Client { ClientId = 1, ClientName = "Client A", ContactPerson = "Person A", Email = "a@a.com", Status = "Active" },
                new Client { ClientId = 2, ClientName = "Client B", ContactPerson = "Person B", Email = "b@b.com", Status = "Active" }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(clients);

            var request = new SearchRequestDto();
            var result = await _clientService.GetAllClientsAsync(request);

            Assert.Equal(2, result.Count);
            Assert.Equal("Client A", result[0].ClientName);
        }

        [Fact]
        public async Task GetClientById_WithValidId_ReturnsClient()
        {
            var client = new Client { ClientId = 1, ClientName = "Client A", ContactPerson = "Person A", Email = "a@a.com", Status = "Active" };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(client);

            var result = await _clientService.GetClientByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Client A", result.ClientName);
        }

        [Fact]
        public async Task GetClientById_WithInvalidId_ReturnsNull()
        {
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Client?)null);

            var result = await _clientService.GetClientByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateClient_ReturnsClient()
        {
            var created = new Client { ClientId = 1, ClientName = "New", ContactPerson = "Person", Email = "a@a.com", Status = "Active" };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Client>(), It.IsAny<int>())).ReturnsAsync(created);

            var request = new CreateClientDto { ClientName = "New", ContactPerson = "Person", Email = "a@a.com", PhoneNumber = "123" };

            var result = await _clientService.CreateClientAsync(request, 1);

            Assert.NotNull(result);
            Assert.Equal("New", result.ClientName);
        }

        [Fact]
        public async Task UpdateClient_WithValidData_ReturnsTrue()
        {
            var existing = new Client { ClientId = 1, ClientName = "Old", ContactPerson = "Person", Email = "a@a.com", Status = "Active" };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Client>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            var request = new UpdateClientDto { ClientName = "Updated", ContactPerson = "Person", Email = "a@a.com", PhoneNumber = "123", Status = "Active" };

            var result = await _clientService.UpdateClientAsync(1, request, 1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteClient_WithValidId_ReturnsTrue()
        {
            var existing = new Client { ClientId = 1, ClientName = "Old", ContactPerson = "Person", Email = "a@a.com", Status = "Active" };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Client>(), 1)).Returns(Task.CompletedTask);

            var result = await _clientService.DeleteClientAsync(1, 1);

            Assert.True(result);
        }
    }
}
