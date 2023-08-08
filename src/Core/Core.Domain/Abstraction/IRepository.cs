namespace Core.Domain.Abstraction;

public interface IRepository<T> where T : IEntity
{
    Task<T> UpsertAsync(T entity);
    Task<T> DeleteAsync(T entity);
}