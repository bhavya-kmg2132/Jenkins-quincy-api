using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Api.Services
{
    /// <summary>
    /// CommonHostedService
    /// </summary>
    public class CacheSyncService : BackgroundService
    {
        private readonly ILogger<CacheSyncService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;


        /// <summary>
        /// CommonHostedService
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="scopeFactory"></param>
        public CacheSyncService(ILogger<CacheSyncService> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._scopeFactory = scopeFactory;
        }

        /// <summary>
        /// ExecuteAsync
        /// </summary>
        /// <param name="stoppingToken3"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            bool runCacheService = Convert.ToBoolean(_configuration["CacheSettings:RunCacheService"]);
            if (!runCacheService) return;

            DateTime userCacheSyncService_lastExecutionTime = DateTime.UtcNow;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    bool runIdentityUserCacheSyncService = Convert.ToBoolean(_configuration["CacheSettings:RunIdentityUserCacheSyncService"]);
                    if (runIdentityUserCacheSyncService)
                    {
                        try
                        {
                            TimeSpan timeSpan = DateTime.UtcNow - userCacheSyncService_lastExecutionTime;
                            int min = (int)timeSpan.TotalMinutes;
                            Int32 intervalTimeInMinutes = Convert.ToInt32(_configuration["CacheSettings:IdentityUserCacheSyncIntervalTimeInMinutes"]);
                            if (min > intervalTimeInMinutes)
                            {
                                _logger.LogInformation("Cache Service initiation started!");

                                await IdentityUserCacheService(stoppingToken);
                                userCacheSyncService_lastExecutionTime = DateTime.UtcNow;

                                _logger.LogInformation("Cache Service initiation completed!");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Cache Sync Service Error - " + ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Cache Sync Service Error - " + ex);
                }
                finally
                {
                    await Task.Delay(30000, stoppingToken);
                }
            }
        }

        /// <summary>
        /// Cache Service
        /// </summary>
        /// <param name="stoppingToken3"></param>
        /// <returns></returns>
        private async Task IdentityUserCacheService(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var identityService = scope.ServiceProvider.GetRequiredService<IIdentityService>();
                await identityService.SyncIdentityUserCacheAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("The IdentityUserCacheService service is Down - Catch block - {0} {1}.", ex.Message, DateTime.UtcNow);
            }
            finally
            {
                //// Every 30 sec
                //await Task.Delay(30000, stoppingToken);
            }
        }
    }
}