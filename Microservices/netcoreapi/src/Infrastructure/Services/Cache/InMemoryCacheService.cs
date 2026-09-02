using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services.Cache
{
    public class InMemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public InMemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<object> GetCacheValueAsync(string key)
        {
            if (_memoryCache.TryGetValue<object>(key, out var cacheResponse))
            {
                return Task.FromResult(cacheResponse);
            }
            return Task.FromResult<object>(null);
        }

        public Task SetCacheValueAsync(string key, object value, TimeSpan expirationTimeFromNow)
        {
            _memoryCache.Set(key, value, absoluteExpirationRelativeToNow: expirationTimeFromNow);
            return Task.CompletedTask;
        }

        public Task RemoveCacheValue(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
        public Task<T> GetCachedValueAsync<T>(string key)
        {
            if (_memoryCache.TryGetValue<T>(key, out var cacheResponse))
            {
                return Task.FromResult(cacheResponse);
            }
            return Task.FromResult<T>(default);
        }

        public async Task<List<String>> GetCacheKeys()
        {
            var cacheKey = new List<String>();

            if (_memoryCache is Microsoft.Extensions.Caching.Memory.MemoryCache memoryCache)
            {
                foreach (object key in memoryCache.Keys)
                {

                    cacheKey.Add(key.ToString());
                }
            }
            await Task.CompletedTask;
            return cacheKey;
        }

    }
}
