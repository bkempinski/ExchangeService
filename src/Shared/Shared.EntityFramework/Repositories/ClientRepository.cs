using Core.Domain.Abstraction.Repositories;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.EntityFramework.Abstraction;

namespace Shared.EntityFramework.Repositories;

public class ClientRepository : RepositoryBase<Client>, IClientRepository
{
    public ClientRepository(IDbContext dbContext) : base(dbContext) { }

    public Task<Client> GetByIpAddressAsync(string ipAddress) => _dbContext.Clients
        .FirstOrDefaultAsync(c => c.IpAddress == ipAddress);
}