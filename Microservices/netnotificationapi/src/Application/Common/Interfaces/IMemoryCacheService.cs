using System;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IMemoryCacheService
    {
        public Task<object> GetCacheValueAsync(string key);

        public Task SetCacheValueAsync(string key, object value, TimeSpan expirationTimeFromNow);

        public Task RemoveCacheValueAsync(string key);

        public object GetCacheValue(string key);

        public void SetCacheValue(string key, object value, TimeSpan expirationTimeFromNow);

        public void RemoveCacheValue(string key);
    }
}