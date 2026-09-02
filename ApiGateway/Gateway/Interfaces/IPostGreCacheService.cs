namespace Gateway.Interface
{
    public interface IPostGreCacheService
    {
        Task<object> GetCacheValueAsync(string key);
    }

}
