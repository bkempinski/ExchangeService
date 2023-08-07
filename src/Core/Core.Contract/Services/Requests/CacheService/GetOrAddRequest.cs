namespace Core.Contract.Services.Requests.CacheService;

public record GetOrAddRequest<T>
{
    public string CacheKey { get; init; }
    public TimeSpan? AbsoluteExpiration { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }
    public Func<Task<T>> ValueFactory { get; init; }
}