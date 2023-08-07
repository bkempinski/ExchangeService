namespace Core.Contract.Services.Responses.CacheService;

public record GetOrAddResponse<T>
{
    public T Value { get; init; }
}