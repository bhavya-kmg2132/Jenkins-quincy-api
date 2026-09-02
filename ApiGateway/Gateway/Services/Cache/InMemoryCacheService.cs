using Gateway.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Services.Cache
{
    public class InMemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public InMemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<object> GetCacheValueAsync(string key)
        {
            if (_memoryCache.TryGetValue<object>(key, out var cacheResponse))
            {
                return cacheResponse;
            }
            await Task.CompletedTask;
            return null;
        }

        public async Task SetCacheValueAsync(string key, object value, TimeSpan expirationTimeFromNow)
        {
            _memoryCache.Set(key, value, absoluteExpirationRelativeToNow: expirationTimeFromNow);
            await Task.CompletedTask;
            return;
        }


        public async Task RemoveCacheValueAsync(string key)
        {
            _memoryCache.Remove(key);
            await Task.CompletedTask;
            return;
        }


        public Task<T> GetCachedValueAsync<T>(string key)
        {
            if (_memoryCache.TryGetValue<T>(key, out var cacheResponse))
            {
                return Task.FromResult(cacheResponse);
            }
            return Task.FromResult<T>(default);
        }
    }
}
