using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;
using Dapper;
using Entities;
using Gateway.Interface;
using Gateway.Interfaces;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using NetAuth.Contract.DataContract.Entities;
using netauthlib;


namespace Services.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly ISqlServerCacheService _sqlServerCacheService;
        private readonly IMemoryCacheService _inMemoryCacheService;
        private readonly INetAuthProvider _netAuthProvider;
        private readonly ILogger<AuthorizationService> _logger;
        private readonly string _cacheKeyForUsers;
        private readonly string _cacheKeyForPermissions;
        private readonly double _inMemoryCacheExpiryHours;
        private readonly int _identityUserCacheSyncIntervalTimeInMinutes;
        private readonly bool _useMemoryCache;
        private readonly bool _useSqlServerCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        private const string ActionPermissionType = "ACTION";

        public string CorrelationId => string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-Id"]) ?
                    Guid.NewGuid().ToString() : _httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-Id"];
        public string RequestId => string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Id"]) ?
                                            Guid.NewGuid().ToString() : _httpContextAccessor.HttpContext?.Request.Headers["X-Request-Id"];
        public string RequestOid => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Oid"] ?? string.Empty}";
        public string RequestUid => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Uid"] ?? string.Empty}";
        public string ApiKey => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Api-Key"] ?? string.Empty}";

        public AuthorizationService(IHostEnvironment environment, IHttpContextAccessor httpContextAccessor, ISqlServerCacheService sqlServerCacheService, INetAuthProvider netAuthProvider, ILogger<AuthorizationService> logger, IMemoryCacheService memoryCacheService, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _inMemoryCacheService = memoryCacheService;
            _sqlServerCacheService = sqlServerCacheService;
            _netAuthProvider = netAuthProvider;
            _cacheKeyForUsers = $"{environment.EnvironmentName}|{configuration["CacheSettings:IdentityUserCacheKey"]}";
            _cacheKeyForPermissions = $"{environment.EnvironmentName}|{configuration["CacheSettings:IdentityUserPermissionCacheKey"]}";
            _useMemoryCache = Convert.ToBoolean(configuration["CacheSettings:UseInMemoryCache"]);
            _useSqlServerCache = Convert.ToBoolean(configuration["CacheSettings:UseSqlServerCache"]);
            _inMemoryCacheExpiryHours = Convert.ToDouble(configuration["CacheSettings:InMemoryCacheExpiryHour"]);
            _identityUserCacheSyncIntervalTimeInMinutes = Convert.ToInt32(configuration["CacheSettings:IdentityUserCacheSyncIntervalTimeInMinutes"]);
        }

        public async Task<bool> SyncIdentityUserCacheAsync()
        {
            try
            {
                List<DeletedInMemoryCacheLog> deletedInMemoryCacheLogs = await GetDeletedInMemoryCacheLogsAsync();
                foreach (DeletedInMemoryCacheLog log in deletedInMemoryCacheLogs)
                {
                    if (log.DeletionTimeInUTC.AddMinutes(_identityUserCacheSyncIntervalTimeInMinutes) < DateTime.UtcNow)
                    {
                        await _inMemoryCacheService.RemoveCacheValueAsync(log.CacheKey);
                        _logger.LogInformation("In-memory cache cleared due to deletion!");
                    }
                }

                // Purge rows old enough to have just been acted on above, so the table doesn't grow
                // unbounded and every cycle isn't re-scanning history that's already been handled.
                DateTime cutoffTimeUtc = DateTime.UtcNow.AddMinutes(-_identityUserCacheSyncIntervalTimeInMinutes);
                await DeleteDeletedInMemoryCacheLogsAsync(cutoffTimeUtc);

                _logger.LogInformation("IdentityManager.SyncIdentityUserCacheAsync Completed!");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"IdentityManager.SyncIdentityUserCacheAsync: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> SyncPermissionsCacheAsync()
        {
            try
            {
                var permissionsObject = await _sqlServerCacheService.GetCacheValueAsync(_cacheKeyForPermissions);

                if (permissionsObject == null)
                {
                    await _inMemoryCacheService.RemoveCacheValueAsync(_cacheKeyForPermissions);
                    _logger.LogInformation("AuthorizationService.SyncPermissionsCacheAsync In memory cache cleared for permissions!");
                    return false;
                }

                List<Permission> permissionsList = JsonSerializer.Deserialize<List<Permission>>(permissionsObject.ToString());

                if (permissionsList == null)
                {
                    permissionsList = await GetPermissionsFromNetAuthAsync();
                    if (permissionsList != null)
                    {
                        await _inMemoryCacheService.SetCacheValueAsync(_cacheKeyForPermissions, permissionsList, TimeSpan.FromHours(_inMemoryCacheExpiryHours));
                    }
                }

                _logger.LogInformation("AuthorizationService.SyncPermissionsCacheAsync Completed!");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("AuthorizationService.SyncPermissionsCacheAsync:" + ex.Message);
                return false;
            }
        }

        public async Task<bool> HasPermissionAsync(string userId, string actionPermissionEndPoint)
        {
            try
            {
                _logger.LogInformation($"AuthorizationService.HasPermissionAsync - In process. UserId: {userId}, Permission: {actionPermissionEndPoint}");

                string gatewayPrefixPattern = @"^/gateway/[^/]+/?";
                string requestedRoute = Regex.Replace(actionPermissionEndPoint, gatewayPrefixPattern, "");

                string versionPrefixPattern = @"^v\d+/";
                requestedRoute = Regex.Replace(requestedRoute, versionPrefixPattern, "", RegexOptions.IgnoreCase);

                var user = await GetIdentityUserAsync(userId);

                if (user == null)
                {
                    return false;
                }

                var allowedApis = _configuration.GetSection("AllowDownstreamApis").Get<string[]>() ?? Array.Empty<string>();
                if (!user.UserPermissions.Any(p => p.ApiName != null && allowedApis.Contains(p.ApiName, StringComparer.OrdinalIgnoreCase)))
                {
                    return false;
                }

                if (!user.UserPermissions.Any(p => p.PermissionType == ActionPermissionType && p.ActionPermissionEndPoint != null && p.ActionPermissionEndPoint.Equals(requestedRoute, StringComparison.OrdinalIgnoreCase)))
                {
                    List<NetAuth.Contract.DataContract.Entities.Permission> allPermissions = await GetPermissionsAsync();
                    if (!allPermissions.Any(p => p.PermissionType == ActionPermissionType && p.ActionPermissionEndPoint != null && p.ActionPermissionEndPoint.Equals(requestedRoute, StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"AuthorizationService.HasPermissionAsync - {ex.Message}. UserId: {userId}, Permission: {actionPermissionEndPoint}");
                throw;
            }
        }

        private async Task<IdentityUser> GetIdentityUserAsync(string userId)
        {
            // Everything downstream keys strictly off this one fully-qualified cache key — never
            // off a field of the fetched IdentityUser (UserId/UserName), which may not equal the
            // identifier the caller looked the user up by. That mismatch is what silently defeated
            // the in-memory user cache before this fix.
            string cacheKey = _cacheKeyForUsers + userId;
            var user = await GetIdentityUserFromCacheAsync(cacheKey);

            if (user == null)
            {
                user = await GetIdentityUserFromNetAuthAsync(userId);
                if (user != null)
                {
                    await UpsertIdentityUserInCacheAsync(cacheKey, user);
                }
            }

            return user;
        }

        private async Task<IdentityUser> GetIdentityUserFromCacheAsync(string cacheKey)
        {
            if (_useMemoryCache)
            {
                var user = await GetIdentityUserFromInMemoryCacheAsync(cacheKey);
                if (user != null) return user;
            }

            if (_useSqlServerCache)
            {
                var cachedUser = await GetIdentityUserFromSqlServerCacheAsync(cacheKey);
                if (cachedUser != null)
                    await UpsertIdentityUserInCacheAsync(cacheKey, cachedUser);

                return cachedUser;
            }

            return null;
        }

        private async Task<IdentityUser> GetIdentityUserFromSqlServerCacheAsync(string cacheKey)
        {
            try
            {
                var cachedUserJson = await _sqlServerCacheService.GetCacheValueAsync(cacheKey);
                if (cachedUserJson != null)
                    return JsonSerializer.Deserialize<IdentityUser>(cachedUserJson.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AuthorizationService.GetIdentityUserFromSqlServerCacheAsync - {ex.Message}");
            }

            return null;
        }

        private async Task<IdentityUser> GetIdentityUserFromInMemoryCacheAsync(string cacheKey)
        {
            try
            {
                return await _inMemoryCacheService.GetCachedValueAsync<IdentityUser>(cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AuthorizationService.GetIdentityUserFromInMemoryCacheAsync - {ex.Message}");
                return null;
            }
        }

        private async Task<IdentityUser> GetIdentityUserFromNetAuthAsync(string userId)
        {
            try
            {
                return await _netAuthProvider.GetIdentityUserByUserName(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AuthorizationService.GetIdentityUserFromNetAuthAsync - {ex.Message}");
            }

            return null;
        }

        private async Task UpsertIdentityUserInCacheAsync(string cacheKey, IdentityUser user)
        {
            if (user == null || !_useMemoryCache) return;

            try
            {
                await _inMemoryCacheService.SetCacheValueAsync(cacheKey, user, TimeSpan.FromHours(_inMemoryCacheExpiryHours));
            }
            catch (Exception ex)
            {
                _logger.LogError($"AuthorizationService.UpsertIdentityUserInCacheAsync - {ex.Message}");
            }
        }

        private async Task<List<Permission>> GetPermissionsAsync()
        {
            try
            {
                var cachedPermissions = await GetPermissionsFromCacheAsync();
                if (cachedPermissions != null) return cachedPermissions;

                var permissions = await GetPermissionsFromNetAuthAsync();
                if (permissions != null)
                    await _inMemoryCacheService.SetCacheValueAsync(_cacheKeyForPermissions, permissions, TimeSpan.FromHours(_inMemoryCacheExpiryHours));

                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError($"AuthorizationService.GetPermissionsAsync - {ex.Message}");
                throw;
            }
        }

        private async Task<List<Permission>> GetPermissionsFromCacheAsync()
        {
            if (_useMemoryCache)
            {
                var cachedPermissions = await GetPermissionsFromInMemoryCacheAsync();
                if (cachedPermissions != null) return cachedPermissions;
            }

            if (_useSqlServerCache)
            {
                var cachedPermissions = await GetPermissionsFromSqlServerCacheAsync();
                if (cachedPermissions != null)
                    await _inMemoryCacheService.SetCacheValueAsync(_cacheKeyForPermissions, cachedPermissions, TimeSpan.FromHours(_inMemoryCacheExpiryHours));

                return cachedPermissions;
            }

            return null;
        }

        private async Task<List<Permission>> GetPermissionsFromSqlServerCacheAsync()
        {
            try
            {
                var cachedPermissionsJson = await _sqlServerCacheService.GetCacheValueAsync(_cacheKeyForPermissions);
                return cachedPermissionsJson != null
                    ? JsonSerializer.Deserialize<List<Permission>>(cachedPermissionsJson.ToString())
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"AuthorizationService.GetPermissionsFromSqlServerCacheAsync - {ex.Message}");
                return null;
            }
        }

        private async Task<List<Permission>> GetPermissionsFromInMemoryCacheAsync()
        {
            try
            {
                return await _inMemoryCacheService.GetCachedValueAsync<List<Permission>>(_cacheKeyForPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AuthorizationService.GetPermissionsFromInMemoryCacheAsync - {ex.Message}");
                return null;
            }
        }

        private async Task<List<Permission>> GetPermissionsFromNetAuthAsync()
        {
            try
            {
                var permissions = await _netAuthProvider.GetPermissionsAsync();
                return permissions?.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"AuthorizationService.GetPermissionsFromNetAuthAsync - {ex.Message}");
            }

            return null;
        }

        private async Task<List<DeletedInMemoryCacheLog>> GetDeletedInMemoryCacheLogsAsync()
        {
            var deletedInMemoryCacheLogs = new List<DeletedInMemoryCacheLog>();

            const string query = @"
        SELECT 
            [CacheKey],
            [DeletionTimeInUTC]
        FROM [dbo].[DeletedInMemoryCacheLog]";

            await using var connection = new SqlConnection(
                _configuration["ConnectionStrings:SqlDBConnection"]);

            await connection.OpenAsync();

            using var reader = await connection.ExecuteReaderAsync(query);

            while (await reader.ReadAsync())
            {
                var log = new DeletedInMemoryCacheLog
                {
                    CacheKey = reader["CacheKey"].ToString(),
                    DeletionTimeInUTC = reader["DeletionTimeInUTC"] != DBNull.Value
                        ? Convert.ToDateTime(reader["DeletionTimeInUTC"])
                        : DateTime.MinValue
                };

                deletedInMemoryCacheLogs.Add(log);
            }

            return deletedInMemoryCacheLogs;
        }

        private async Task DeleteDeletedInMemoryCacheLogsAsync(DateTime cutoffTimeUtc)
        {
            try
            {
                const string query = @"
                    DELETE FROM [dbo].[DeletedInMemoryCacheLog]
                    WHERE [DeletionTimeInUTC] <= @CutoffTimeUtc";

                await using var connection = new SqlConnection(
                    _configuration["ConnectionStrings:SqlDBConnection"]);

                await connection.OpenAsync();

                await connection.ExecuteAsync(
                    query,
                    new
                    {
                        CutoffTimeUtc = cutoffTimeUtc
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"AuthorizationService.DeleteDeletedInMemoryCacheLogsAsync: {ex.Message}");
            }
        }
    }
}
