using System;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IRedisCacheService
    {
        Task<T> GetCachedValueAsync<T>(string key);

        public Task<object> GetCacheValueAsync(string key);

        public Task SetCacheValueAsync(string key, object value, TimeSpan expirationTimeFromNow);

        public Task RemoveCacheValue(string key);
    }
}