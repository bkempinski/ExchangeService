﻿using Core.Contract.Services.Requests.CacheService;
using Core.Contract.Services.Responses.CacheService;

namespace Core.Services.Abstraction;

public interface ICacheService
{
    Task<GetOrAddResponse<T>> GetOrAddInMemoryAsync<T>(GetOrAddRequest<T> request);
    Task<GetOrAddResponse<T>> GetOrAddDistributedAsync<T>(GetOrAddRequest<T> request);
    Task<SetValueResponse<T>> SetValueInMemoryAsync<T>(SetValueRequest<T> request);
    Task<SetValueResponse<T>> SetValueDistributedAsync<T>(SetValueRequest<T> request);
}