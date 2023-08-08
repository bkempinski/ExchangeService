namespace Core.Contract.Services.Requests.CacheService;

public record SetValueRequest<T>
{
    public string CacheKey { get; init; }
    public TimeSpan? AbsoluteExpiration { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }
    public T Value { get; init; }
}