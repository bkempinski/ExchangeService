using Core.Contract.Services.Requests.CacheService;
using Core.Contract.Services.Responses.CacheService;
using Core.Services.Abstraction;

namespace Test.Core.Services.Fakes;

public class FakeCacheService : ICacheService
{
    public ICacheService Object => this;

    public async Task<GetOrAddResponse<T>> GetOrAddInMemoryAsync<T>(GetOrAddRequest<T> request) =>
        new GetOrAddResponse<T> { Value = await request.ValueFactory() };

    public async Task<GetOrAddResponse<T>> GetOrAddDistributedAsync<T>(GetOrAddRequest<T> request) =>
        new GetOrAddResponse<T> { Value = await request.ValueFactory() };

    public Task<SetValueResponse<T>> SetValueInMemoryAsync<T>(SetValueRequest<T> request) =>
        Task.FromResult(new SetValueResponse<T> { Value = request.Value });

    public Task<SetValueResponse<T>> SetValueDistributedAsync<T>(SetValueRequest<T> request) =>
        Task.FromResult(new SetValueResponse<T> { Value = request.Value });
}
