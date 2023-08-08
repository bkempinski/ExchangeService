using Core.Domain.Abstraction;
using Microsoft.EntityFrameworkCore;
using Shared.EntityFramework.Abstraction;

namespace Shared.EntityFramework;

public abstract class RepositoryBase<T> : IRepository<T> where T : class, IEntity
{
    protected readonly IDbContext _dbContext;

    public RepositoryBase(IDbContext dbContext) => 
        _dbContext = dbContext;

    public async Task<T> UpsertAsync(T entity)
    {
        _dbContext.Entry(entity).State = entity.Id == 0 ? 
            EntityState.Added : EntityState.Modified;

        await _dbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<T> DeleteAsync(T entity)
    {
        if (entity.Id != 0)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;

            await _dbContext.SaveChangesAsync();

            return entity;
        }

        return null;
    }
}