using Core.Domain.Abstraction;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Shared.EntityFramework.Abstraction;

public interface IDbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Trade> Trades { get; set; }

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}