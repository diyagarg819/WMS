using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly WMSDbContext _context;

        public ClientRepository(WMSDbContext context)
        {
            _context = context;
        }

        public async Task<List<Client>> GetAllClientsAsync(string? searchTerm)
        {
            var query = _context.Clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                bool isNumeric = int.TryParse(term, out int searchId);

                if (isNumeric)
                {
                    // Search by exact ID, or see if it matches the phone number
                    query = query.Where(c => 
                        c.ClientId == searchId || 
                        (c.ClientPhoneNumber != null && c.ClientPhoneNumber.ToString().Contains(term)));
                }
                else
                {
                    // Search text fields
                    query = query.Where(c => 
                        c.ClientName.ToLower().Contains(term) ||
                        (c.ClientLocation != null && c.ClientLocation.ToLower().Contains(term)));
                }
            }

            return await query
                .OrderBy(c => c.ClientName)
                .ToListAsync();
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.ClientId == id);
        }

        public async Task<Client> AddClientAsync(Client client, int createdByUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Clients.AddAsync(client);
                await _context.SaveChangesAsync();

                await _context.AuditLogs.AddAsync(new AuditLog
                {
                    Action = "Create Client",
                    EntityName = "Client",
                    RecordId = client.ClientId,
                    CreatedBY = createdByUserId,
                    CreatedOn = DateTime.Now
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return client;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateClientAsync(Client client, int updatedByUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Clients.Update(client);
                
                await _context.AuditLogs.AddAsync(new AuditLog
                {
                    Action = "Update Client",
                    EntityName = "Client",
                    RecordId = client.ClientId,
                    CreatedBY = updatedByUserId,
                    CreatedOn = DateTime.Now
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteClientAsync(Client client, int deletedByUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int recordId = client.ClientId;
                _context.Clients.Remove(client);
                
                await _context.AuditLogs.AddAsync(new AuditLog
                {
                    Action = "Delete Client",
                    EntityName = "Client",
                    RecordId = recordId,
                    CreatedBY = deletedByUserId,
                    CreatedOn = DateTime.Now
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
