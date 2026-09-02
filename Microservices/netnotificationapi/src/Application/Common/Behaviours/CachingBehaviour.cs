using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Attributes;
using Application.Common.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours
{
    public class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger _logger;
        private readonly bool _useMemoryCache;
        private IMemoryCacheService _memoryCacheService { get; }

        private string _cachePrefix;
        public CachingBehaviour(IWebHostEnvironment env, IConfiguration configuration, ILogger<CachingBehaviour<TRequest, TResponse>> logger, IMemoryCacheService memoryCacheService = null)
        {
            _logger = logger;
            _useMemoryCache = Convert.ToBoolean(configuration["CacheSettings:UseInMemoryCache"]);
            _cachePrefix = $"{env.EnvironmentName}|{configuration["Api:internal_name"]}|";

            // Initialize cache services based on the configuration
            if (_useMemoryCache)
            {
                _memoryCacheService = memoryCacheService;
            }
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var cacheQuery = typeof(TRequest).GetCustomAttribute<CacheQueryResponseAttribute>();
            var commandRequest = typeof(TRequest).GetCustomAttribute<InvalidateCacheAttribute>();

            if (cacheQuery != null)
            {
                return await HandleQuery(cacheQuery, request, next);
            }

            if (commandRequest != null && commandRequest.Queries != Type.EmptyTypes)
            {
                await HandleCommand(commandRequest);
            }

            return await next();
        }

        private async Task<TResponse> HandleQuery(CacheQueryResponseAttribute cacheQuery, TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            var cacheKey = _cachePrefix + (string.IsNullOrEmpty(cacheQuery.CacheKey)
                                           ? CacheHelper.GenerateCacheKeyFromRequest(request)
                                           : cacheQuery.CacheKey);



            Object cachedResponse = null;

            if (_useMemoryCache && cacheQuery.CacheServer.Equals(CacheServerType.InMemory))
            {
                cachedResponse = await _memoryCacheService.GetCacheValueAsync(cacheKey);
            }

            if (cachedResponse != null)
            {
                _logger.LogInformation($"Request {typeof(TRequest).Name} served from cache");
                var data = (TResponse)cachedResponse;
                return data;
            }

            var actualResponse = await next();
            if (_useMemoryCache && cacheQuery.CacheServer.Equals(CacheServerType.InMemory))
            {
                await _memoryCacheService.SetCacheValueAsync(cacheKey, actualResponse, cacheQuery.TimeSpanForCacheInvalidation);
            }

            return actualResponse;
        }

        private async Task HandleCommand(InvalidateCacheAttribute commandRequest)
        {
            foreach (var type in commandRequest.Queries)
            {
                var queryType = type.GetCustomAttribute<CacheQueryResponseAttribute>();
                var key = _cachePrefix + (string.IsNullOrEmpty(queryType.CacheKey)
                    ? CacheHelper.GenerateCacheKeyFromRequest(Activator.CreateInstance(type))
                    : queryType.CacheKey);

                if (_useMemoryCache && queryType.CacheServer.Equals(CacheServerType.InMemory))
                {
                    await _memoryCacheService.RemoveCacheValueAsync(key);
                }
            }
        }
    }

    public static class CacheHelper
    {
        public static string GenerateCacheKeyFromRequest(object request)
        {
            var key = new StringBuilder();
            key.Append($"{request.GetType().Name}|");
            foreach (var property in request.GetType().GetProperties())
            {
                key.Append($"{property.Name}|{property.GetValue(request)}|");
            }
            return key.ToString();
        }
    }
}
