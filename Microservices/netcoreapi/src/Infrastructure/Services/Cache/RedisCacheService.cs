using System;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services.Cache
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _distributedCache;

        public RedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<object> GetCacheValueAsync(string key)
        {
            var cached = await _distributedCache.GetStringAsync(key);
            if (cached != null)
                return await Task.FromResult(JsonSerializer.Deserialize<object>(cached));

            return await Task.FromResult<object>(null);
        }

        public async Task RemoveCacheValue(string key)
        {
            if (key != null)
            {
                await _distributedCache.RemoveAsync(key);
            }
        }

        public async Task SetCacheValueAsync(string key, object value, TimeSpan expirationTimeFromNow)
        {
            var serializedResponse = JsonSerializer.Serialize(value);

            await _distributedCache.SetStringAsync(key, serializedResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTimeFromNow
            });
        }
        public async Task<T> GetCachedValueAsync<T>(string key)
        {
            var cachedValue = await _distributedCache.GetStringAsync(key);

            if (cachedValue != null)
            {
                return JsonSerializer.Deserialize<T>(cachedValue);
            }

            return default(T);
        }
    }
}