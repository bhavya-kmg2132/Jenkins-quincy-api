using System.Text.RegularExpressions;
using Gateway.Interface;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Services.Cache
{
    public class PostGreCacheService : IPostGreCacheService
    {
        private ILogger<PostGreCacheService> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly IConfiguration _configuration;

        private string _rebuildKey;

        public PostGreCacheService(IDistributedCache distributedCache, ILogger<PostGreCacheService> logger, IHostEnvironment env, IConfiguration configuration)
        {
            this._distributedCache = distributedCache;
            this._logger = logger;
            this._rebuildKey = $"{env.EnvironmentName}|CachingRebuildingServer";
            this._configuration = configuration;
        }

        public async Task<object> GetCacheValueAsync(string key)
        {
            try
            {
                var isCacheRebuilding = await _distributedCache.GetStringAsync(_rebuildKey);
                if (isCacheRebuilding == "true")
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }

                var cached = await _distributedCache.GetStringAsync(key);
                if (cached != null)
                {

                    // Recursive regex pattern to capture JSON object
                    string pattern = @"(\{(?:[^{}]|(?<o>\{)|(?<-o>\}))+(?(o)(?!))\}|\[(?:[^\[\]]|(?<a>\[)|(?<-a>\]))+(?(a)(?!))\])";

                    Match match = Regex.Match(cached, pattern, RegexOptions.Singleline);

                    if (match.Success)
                    {
                        cached = match.Value;
                    }

                    return JsonConvert.DeserializeObject(cached, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"PostGresCacheService.GetCacheValueAsync : Error fetching cache value from PostGreSQL key : {key} exception : {ex.Message}");
            }

            return null;
        }

    }
}
