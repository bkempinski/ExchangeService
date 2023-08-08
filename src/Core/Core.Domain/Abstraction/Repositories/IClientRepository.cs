using Core.Domain.Entities;

namespace Core.Domain.Abstraction.Repositories;

public interface IClientRepository : IRepository<Client>
{
    Task<Client> GetByIpAddressAsync(string ipAddress);
}