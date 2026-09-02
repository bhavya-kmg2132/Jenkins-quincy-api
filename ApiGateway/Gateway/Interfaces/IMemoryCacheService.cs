namespace Gateway.Interfaces
{
    public interface IMemoryCacheService
    {
        Task<object> GetCacheValueAsync(string key);

        Task SetCacheValueAsync(string key, object value, TimeSpan expirationTimeFromNow);

        Task RemoveCacheValueAsync(string key);

        Task<T> GetCachedValueAsync<T>(string key);
    }
}