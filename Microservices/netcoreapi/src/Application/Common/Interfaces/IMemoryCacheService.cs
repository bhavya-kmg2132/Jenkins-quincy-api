using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IMemoryCacheService
    {
        Task<T> GetCachedValueAsync<T>(string key);

        Task<object> GetCacheValueAsync(string key);

        Task SetCacheValueAsync(string key, object value, TimeSpan expirationTimeFromNow);

        Task RemoveCacheValue(string key);

        Task<List<String>> GetCacheKeys();
    }
}