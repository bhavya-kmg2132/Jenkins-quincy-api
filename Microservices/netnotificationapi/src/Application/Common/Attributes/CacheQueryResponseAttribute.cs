using System;
using Domain.Enums;


namespace Application.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CacheQueryResponseAttribute : Attribute
    {

        public string CacheKey;
        public int Duration;
        public TimeSpan TimeSpanForCacheInvalidation = TimeSpan.FromMilliseconds(60000);
        public CacheServerType CacheServer = CacheServerType.InMemory;

        public CacheQueryResponseAttribute()
        {
        }

        public CacheQueryResponseAttribute(string cacheKey)
        {
            CacheKey = cacheKey;
        }

        public CacheQueryResponseAttribute(int duration)
        {
            Duration = duration;
            TimeSpanForCacheInvalidation = TimeSpan.FromMilliseconds(duration);
        }
        public CacheQueryResponseAttribute(int duration, CacheServerType cacheServer)
        {
            Duration = duration;
            TimeSpanForCacheInvalidation = TimeSpan.FromMilliseconds(duration);
            CacheServer = cacheServer;
        }

        public CacheQueryResponseAttribute(string cacheKey, int duration)
        {
            CacheKey = cacheKey;
            Duration = duration;
            TimeSpanForCacheInvalidation = TimeSpan.FromMilliseconds(duration);
        }
    }
}
