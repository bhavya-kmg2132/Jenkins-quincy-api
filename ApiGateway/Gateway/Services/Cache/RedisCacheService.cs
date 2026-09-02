using System.Text.Json;
using Gateway.Interface;
using Microsoft.Extensions.Caching.Distributed;

namespace Services.Cache
{
    public class RedisCacheService : IRedisCacheService
    {
        private ILogger<RedisCacheService> _logger;
        private readonly IDistributedCache _distributedCache;
        private string _redisRebuildKey;

        public RedisCacheService(IDistributedCache distributedCache, ILogger<RedisCacheService> logger, IHostEnvironment env)
        {
            this._distributedCache = distributedCache;
            this._logger = logger;
            this._redisRebuildKey = $"{env.EnvironmentName}|CachingRebuildingServer";

        }

        public async Task<object> GetCacheValueAsync(string key)
        {
            try
            {
                var cached = await _distributedCache.GetStringAsync(key);
                if (cached != null)
                {
                    return JsonSerializer.Deserialize<object>(cached);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"RedisCacheService.GetCacheValueAsync : Error fetching cache value from Redis key : {key} exception : {ex.Message}");
            }

            return null;
        }

    }
}
