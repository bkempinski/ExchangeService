namespace Core.Contract.Services.Responses.CacheService;

public class SetValueResponse<T>
{
    public T Value { get; init; }
}