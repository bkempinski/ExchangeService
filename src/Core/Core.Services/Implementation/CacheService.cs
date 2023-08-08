using Core.Contract.Services.Requests.CacheService;
using Core.Contract.Services.Responses.CacheService;
using Core.Services.Abstraction;
using Medallion.Threading;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Core.Services.Implementation;

public class CacheService : ICacheService
{
    private static TimeSpan _distributedLockAcquireTimeout = TimeSpan.FromSeconds(10);

    private readonly ILogger<CacheService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly IDistributedLockProvider _distributedLock;

    public CacheService
        (
            ILogger<CacheService> logger,
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            IDistributedLockProvider distributedLock
        ) => (_logger, _memoryCache, _distributedCache, _distributedLock)
            = (logger, memoryCache, distributedCache, distributedLock);

    public async Task<GetOrAddResponse<T>> GetOrAddInMemoryAsync<T>(GetOrAddRequest<T> request)
    {
        _logger.LogDebug($"CacheService -> GetOrAddInMemoryAsync -> Request: {request}");

        if (string.IsNullOrEmpty(request.CacheKey))
            throw new Domain.Exceptions.ArgumentNullException(nameof(request.CacheKey));

        var value = await _memoryCache.GetOrCreateAsync(request.CacheKey, async cacheEntry =>
        {
            if (request.AbsoluteExpiration.HasValue)
                cacheEntry.AbsoluteExpirationRelativeToNow = request.AbsoluteExpiration.Value;
            else if (request.SlidingExpiration.HasValue)
                cacheEntry.SlidingExpiration = request.SlidingExpiration.Value;
            else
                _logger.LogWarning($"Cache expiration not set - CacheKey: {request.CacheKey}");

            if (request.ValueFactory != null)
                return await request.ValueFactory();
            else
                throw new ArgumentNullException(nameof(request.ValueFactory));
        });

        return new GetOrAddResponse<T>
        {
            Value = value
        };
    }

    public async Task<GetOrAddResponse<T>> GetOrAddDistributedAsync<T>(GetOrAddRequest<T> request)
    {
        _logger.LogDebug($"CacheService -> GetOrAddDistributedAsync -> Request: {request}");

        if (string.IsNullOrEmpty(request.CacheKey))
            throw new Domain.Exceptions.ArgumentNullException(nameof(request.CacheKey));

        await using (var lockHandle = await _distributedLock.CreateLock(request.CacheKey).TryAcquireAsync(_distributedLockAcquireTimeout))
        {
            if (lockHandle != null)
            {
                var cacheObjectBytes = await _distributedCache.GetAsync(request.CacheKey);

                if (cacheObjectBytes != null)
                {
                    var value = DeserializerCacheObject<T>(cacheObjectBytes);

                    return new GetOrAddResponse<T>
                    {
                        Value = value
                    };
                }
                else if (request.ValueFactory != null)
                {
                    var value = await request.ValueFactory();
                    var cacheEntryOptions = new DistributedCacheEntryOptions();

                    if (request.AbsoluteExpiration.HasValue)
                        cacheEntryOptions.AbsoluteExpirationRelativeToNow = request.AbsoluteExpiration.Value;
                    else if (request.SlidingExpiration.HasValue)
                        cacheEntryOptions.SlidingExpiration = request.SlidingExpiration.Value;
                    else
                        _logger.LogWarning($"Cache expiration not set - CacheKey: {request.CacheKey}");

                    cacheObjectBytes = SerializerCacheObject(value);

                    await _distributedCache.SetAsync(request.CacheKey, cacheObjectBytes, cacheEntryOptions);

                    return new GetOrAddResponse<T>
                    {
                        Value = value
                    };
                }
                else
                    throw new ArgumentNullException(nameof(request.ValueFactory));
            }
            else
                throw new ArgumentNullException(nameof(lockHandle));
        }
    }

    public Task<SetValueResponse<T>> SetValueInMemoryAsync<T>(SetValueRequest<T> request)
    {
        _logger.LogDebug($"CacheService -> SetValueInMemoryAsync -> Request: {request}");

        if (string.IsNullOrEmpty(request.CacheKey))
            throw new Domain.Exceptions.ArgumentNullException(nameof(request.CacheKey));

        var cacheEntryOptions = new MemoryCacheEntryOptions();

        if (request.AbsoluteExpiration.HasValue)
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = request.AbsoluteExpiration.Value;
        else if (request.SlidingExpiration.HasValue)
            cacheEntryOptions.SlidingExpiration = request.SlidingExpiration.Value;
        else
            _logger.LogWarning($"Cache expiration not set - CacheKey: {request.CacheKey}");

        _memoryCache.Set(request.CacheKey, request.Value, cacheEntryOptions);

        return Task.FromResult(new SetValueResponse<T>
        {
            Value = request.Value
        });
    }

    public async Task<SetValueResponse<T>> SetValueDistributedAsync<T>(SetValueRequest<T> request)
    {
        _logger.LogDebug($"CacheService -> SetValueDistributedAsync -> Request: {request}");

        if (string.IsNullOrEmpty(request.CacheKey))
            throw new Domain.Exceptions.ArgumentNullException(nameof(request.CacheKey));

        await using (var lockHandle = await _distributedLock.CreateLock(request.CacheKey).TryAcquireAsync(_distributedLockAcquireTimeout))
        {
            if (lockHandle != null)
            {
                var cacheEntryOptions = new DistributedCacheEntryOptions();

                if (request.AbsoluteExpiration.HasValue)
                    cacheEntryOptions.AbsoluteExpirationRelativeToNow = request.AbsoluteExpiration.Value;
                else if (request.SlidingExpiration.HasValue)
                    cacheEntryOptions.SlidingExpiration = request.SlidingExpiration.Value;
                else
                    _logger.LogWarning($"Cache expiration not set - CacheKey: {request.CacheKey}");

                var cacheObjectBytes = SerializerCacheObject(request.Value);

                await _distributedCache.SetAsync(request.CacheKey, cacheObjectBytes, cacheEntryOptions);

                return new SetValueResponse<T>
                {
                    Value = request.Value
                };
            }
            else
                throw new ArgumentNullException(nameof(lockHandle));
        }
    }

    private byte[] SerializerCacheObject<T>(T cacheObject)
    {
        if (cacheObject == null)
            return null;

        var jsonString = JsonSerializer.Serialize(cacheObject);

        return Encoding.UTF8.GetBytes(jsonString);
    }

    private T DeserializerCacheObject<T>(byte[] cacheObjectBytes)
    {
        if (cacheObjectBytes == null || cacheObjectBytes.Length <= 0)
            return default;

        var jsonString = Encoding.UTF8.GetString(cacheObjectBytes);

        return JsonSerializer.Deserialize<T>(jsonString);
    }
}