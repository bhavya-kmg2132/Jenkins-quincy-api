namespace Gateway.Interface
{
    public interface IRedisCacheService
    {
        Task<object> GetCacheValueAsync(string key);
    }

}
