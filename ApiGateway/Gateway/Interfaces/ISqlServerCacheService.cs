namespace Gateway.Interface
{
    public interface ISqlServerCacheService
    {
        Task<object> GetCacheValueAsync(string key);
    }

}
