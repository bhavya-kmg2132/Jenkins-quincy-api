using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Dapper.Extensions;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetAuth.Contract.DataContract.Entities;
using netauthlib;

namespace Infrastructure.DataAccess
{
    public class IdentityManager : IIdentityManager
    {
        private readonly ILogger<IdentityManager> _logger;
        private readonly IConfiguration _configuration;
        private string _cacheKeyForUsers;
        private string _cacheTagForUsers;
        private string _cacheKeyForPermissions;
        private string _cacheKeyForAllPermissions;
        private double _cacheExpiryHours;
        private double _inMemoryCacheExpiryHours;
        private double _inMemoryPermissionCacheExpiryHours;
        private double _permissionsCacheExpiryHour;

        private string _cachePrefix;
        private readonly IMemoryCacheService _inMemoryCacheService;
        private readonly HybridCache _hybridCache;

        private readonly INetAuthProvider _netAuthUser;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int _identityUserCacheSyncIntervalTimeInMinutes;

        public string CorrelationId => string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-Id"]) ?
                                    Guid.NewGuid().ToString() : _httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-Id"];
        public string RequestId => string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Id"]) ?
                                            Guid.NewGuid().ToString() : _httpContextAccessor.HttpContext?.Request.Headers["X-Request-Id"];
        public string RequestOid => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Oid"] ?? string.Empty}";

        public string RequestUid => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Uid"] ?? string.Empty}";
        public string ApiKey => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Api-Key"] ?? string.Empty}";

        // "Already reset today" latch for the nightly cache reset.
        //
        // Static, not an instance field: IdentityManager is transient and CacheSyncService builds a
        // fresh DI scope on every tick, so an instance field would be re-created each time and could
        // never remember that today's reset had already run.
        private static readonly object CacheSyncStateLock = new object();
        private static DateTime _lastNightlyResetDateUtc = DateTime.MinValue;

        /// <summary>
        /// Instantiation of IdentityManager class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public IdentityManager(IWebHostEnvironment env, IConfiguration configuration, ILogger<IdentityManager> logger, INetAuthProvider netAuthUser, IHttpContextAccessor httpContextAccessor, HybridCache hybridCache, IMemoryCacheService memoryCacheService = null)
        {

            this._logger = logger;
            this._configuration = configuration;
            this._hybridCache = hybridCache;
            this._inMemoryCacheService = memoryCacheService;
            _cachePrefix = $"{env.EnvironmentName}|{configuration["Api:internal_name"]}|";

            _inMemoryCacheExpiryHours = Convert.ToDouble(configuration["CacheSettings:InMemoryCacheExpiryHour"]);
            _cacheExpiryHours = Convert.ToDouble(configuration["CacheSettings:CacheExpiryHour"]);
            _inMemoryPermissionCacheExpiryHours = Convert.ToDouble(configuration["CacheSettings:InMemoryPermissionsCacheExpiryHour"]);
            _permissionsCacheExpiryHour = Convert.ToDouble(configuration["CacheSettings:PermissionsCacheExpiryHour"]);

            this._cacheKeyForUsers = _cachePrefix + "IdentityManager|User|";
            this._cacheTagForUsers = _cachePrefix + "IdentityManager|User";
            this._cacheKeyForPermissions = _cachePrefix + "IdentityManager|GetPermissions";
            this._cacheKeyForAllPermissions = _cachePrefix + "IdentityManager|GetAllPermissions";

            this._netAuthUser = netAuthUser;
            this._httpContextAccessor = httpContextAccessor;
            this._identityUserCacheSyncIntervalTimeInMinutes = Convert.ToInt32(_configuration["CacheSettings:IdentityUserCacheSyncIntervalTimeInMinutes"]);

        }

        public async Task<bool> SyncIdentityUserCacheAsync()
        {
            await NightlyCacheResetAsync();

            try
            {
                List<DeletedInMemoryCacheLog> deletedInMemoryCacheLogs = await GetdeletedInMemoryCacheLogsAsync();
                foreach (DeletedInMemoryCacheLog log in deletedInMemoryCacheLogs)
                {
                    if (log.DeletionTimeInUTC.AddMinutes(_identityUserCacheSyncIntervalTimeInMinutes) < DateTime.UtcNow)
                    {
                        await _inMemoryCacheService.RemoveCacheValue(log.CacheKey);
                        _logger.LogInformation("In-memory cache cleared due to deletion!");

                    }
                }

                _logger.LogInformation("IdentityManager.SyncIdentityUserCacheAsync Completed!");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"IdentityManager.SyncIdentityUserCacheAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> NightlyCacheResetAsync()
        {
            try
            {
                int nightlyCacheResetHour = Convert.ToInt32(_configuration["CacheSettings:NightlyCacheResetHour"]);
                DateTime currentTimeInUTC = DateTime.UtcNow;
                bool resetIsDue;

                // Fire once per day, during the configured UTC hour.
                //
                // The previous test was "UtcNow >= UtcNow.Date.AddHours(resetHour)". Because .Date
                // truncates to midnight, that compared against 02:00 *today* - a time already past
                // for most of the day - so it was true on every tick from 02:00 through to midnight.
                // Combined with the ~6 minute sync cadence that flushed the entire identity cache
                // roughly 220 times a day instead of once, making the configured cache lifetimes
                // meaningless and masking staleness bugs.
                lock (CacheSyncStateLock)
                {
                    resetIsDue = currentTimeInUTC.Hour == nightlyCacheResetHour
                                 && _lastNightlyResetDateUtc != currentTimeInUTC.Date;

                    if (resetIsDue)
                        _lastNightlyResetDateUtc = currentTimeInUTC.Date;
                }

                if (resetIsDue)
                {
                    _logger.LogInformation(
                        $"IdentityManager.NightlyCacheResetAsync - running daily reset for {currentTimeInUTC:yyyy-MM-dd} (UTC).");

                    await _hybridCache.RemoveByTagAsync(_cacheTagForUsers);
                    await _hybridCache.RemoveAsync(_cacheKeyForPermissions);

                    // The cache has just been cleared wholesale, so queued per-key invalidation rows
                    // are redundant - clear the log table as housekeeping. This deletes rows for every
                    // process, so it is only safe because the latch above guarantees it runs once a
                    // day rather than continuously.
                    await DeleteDeletedInMemoryCacheLogsAsync();
                }
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError($"IdentityManager.SyncIdentityUserCacheAsync:{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Removes every row from the invalidation log. Called by the nightly reset, which has just
        /// cleared the cache wholesale, so queued per-key invalidations are redundant for this
        /// process. Query comes from the Auth query XML, so it works on SQL Server and PostgreSQL.
        /// </summary>
        private async Task DeleteDeletedInMemoryCacheLogsAsync()
        {
            try
            {
                string query = "DELETE FROM [dbo].[DeletedInMemoryCacheLog];";
                await SqlHelper.ExecuteNonQueryAsync(this._configuration["ConnectionStrings:SqlDBConnection"], CommandType.Text, query);
            }
            catch (Exception ex)
            {
                // Housekeeping only - never fail the nightly reset because the cleanup could not run.
                _logger.LogError($"IdentityManager.DeleteDeletedInMemoryCacheLogsAsync - {ex.Message}");
            }
        }


        public async Task<IdentityUser> GetIdentityUserAsync(string userName_userId_userOid)
        {
            try
            {
                var tags = new List<string> { _cacheTagForUsers };
                var entryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(_cacheExpiryHours),
                    LocalCacheExpiration = TimeSpan.FromHours(_inMemoryCacheExpiryHours),
                };

                return await this._hybridCache.GetOrCreateAsync<IdentityUser>(_cacheKeyForUsers + userName_userId_userOid,
                   async _ =>
                   {
                       IdentityUser userData = await GetIdentityUserFromNetAuthAsync(userName_userId_userOid);
                       if (userData == null)
                       {
                           return null;
                       }
                       var dateNow = DateTime.UtcNow;
                       DateTime calculatedExpirationTime = userData.CacheTimeStamp.AddHours(_cacheExpiryHours);
                       TimeSpan expirationDuration = calculatedExpirationTime > DateTime.UtcNow ? calculatedExpirationTime - dateNow : TimeSpan.Zero;

                       DateTime calculatedLocalExpirationTime = userData.CacheTimeStamp.AddHours(_inMemoryCacheExpiryHours);
                       TimeSpan localExpirationDuration = calculatedExpirationTime > DateTime.UtcNow ? calculatedExpirationTime - dateNow : TimeSpan.Zero;

                       entryOptions = new HybridCacheEntryOptions
                       {
                           Expiration = expirationDuration,
                           LocalCacheExpiration = localExpirationDuration
                       };

                       return userData;
                   }
                   , entryOptions,
                   tags);

            }
            catch (Exception ex)
            {
                _logger.LogError("IdentityManager.GetIdentityUserAsync - " + ex.Message);
                throw;
            }
        }

        public async Task<List<NetAuth.Contract.DataContract.Dto.UserDto>> GetUsersAsync()
        {
            try
            {
                _logger.LogInformation("IdentityManager.GetUsersAsync - In process");

                List<NetAuth.Contract.DataContract.Dto.UserDto> users = await AuthGetUsersFromDbAsync();

                _logger.LogInformation("IdentityManager.GetUsersAsync - Completed");
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError("IdentityManager.GetUsersAsync - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List<Domain.Entities.User></returns>
        public async Task<List<NetAuth.Contract.DataContract.Dto.UserDto>> AuthGetUsersFromDbAsync()
        {
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("IdentityManager.AuthGetUsersFromDbAsync - In process");
                List<NetAuth.Contract.DataContract.Dto.UserDto> users = new List<NetAuth.Contract.DataContract.Dto.UserDto>();

                //Step 2: Call NetAuth api for GetUsers
                users = await _netAuthUser.GetUsersAsync();

                //Step 3: if response is null, then return null
                if (users == null || users.Count == 0)
                {
                    _logger.LogInformation("UserDataAccess.GetUsers - users NotFound.");
                    return null;
                }

                //Step 4: Logging Information Completed
                _logger.LogInformation("IdentityManager.AuthGetUsersFromDbAsync - Completed");

                //Step 5: Return user
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError("IdentityManager.AuthGetUsersFromDbAsync - " + ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets User from DB  
        /// first check based on username then on userid then on useroid 
        /// this method is for identification purpose only
        /// </summary>
        /// <param name="userName_userId_userOid"></param>
        /// <returns></returns>
        public async Task<IdentityUser> GetIdentityUserFromNetAuthAsync(string userName_userId_userOid)
        {
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("IdentityManager.GetIdentityUserFromNetAuthAsync - In process");

                IdentityUser identityUser = new IdentityUser();


                //Step 2: Call NetAuth api for GetUsers
                var netAuthApiResponse = await _netAuthUser.GetIdentityUserByUserName(userName_userId_userOid);

                //Step 3: if response is null, then return null
                if (netAuthApiResponse == null)
                {
                    _logger.LogInformation("UserDataAccess.GetIdentityUserFromNetAuthAsync - user NotFound for userId: " + userName_userId_userOid);
                    return null;
                }

                var netAuthUser = System.Text.Json.JsonSerializer.Serialize(netAuthApiResponse);

                //Step 4: Deserialize output string to user object
                var response = JsonObject.Parse(netAuthUser);
                identityUser = System.Text.Json.JsonSerializer.Deserialize<IdentityUser>(response.ToString());


                //Step 5: Logging Information - Completed
                _logger.LogInformation("IdentityManager.GetIdentityUserFromNetAuthAsync - Completed");

                //Step 6: Return IdentityUser
                return identityUser;
            }
            catch (Exception ex)
            {
                _logger.LogError($"IdentityManager.GetIdentityUserFromDbAsync - {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// check whether user has permission or not
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionValue"></param>
        /// <returns>bool</returns>
        public async Task<bool> AuthHasRequestPermissionAsync(string userId, string permissionValue)
        {
            try
            {
                _logger.LogInformation("IdentityManager.AuthHasRequestPermissionAsync - In process. UserId:" + userId + " permissionValue:" + permissionValue);
                IdentityUser user = null;

                //Step 2: Get User By userId
                user = await GetIdentityUserAsync(userId);

                bool hasRequestPermission = user.UserPermissions.Any(p => p.PermissionValue.Equals(permissionValue));

                if (!hasRequestPermission)
                {
                    List<NetAuth.Contract.DataContract.Entities.Permission> permissions = await GetPermissionsAsync();
                    if (permissions.Any(p => p.PermissionValue.Equals(permissionValue)))
                        return false;
                    else
                        return true;
                }
                else
                {
                    return hasRequestPermission;
                }
                ;
            }
            catch (Exception ex)
            {
                _logger.LogError("IdentityManager.AuthHasRequestPermissionAsync - " + ex.Message + " UserId:" + userId + " permissionValue: " + permissionValue);
                throw;
            }
        }

        /// <summary>
        /// Auth Reset User Cache
        /// </summary>
        /// <returns>Task</returns>
        private async System.Threading.Tasks.Task AuthResetUserCache(List<string> userIds = null)
        {
            try
            {
                // Step 1: Logging Information: In process
                _logger.LogInformation("IdentityManager.AuthResetUserCache - In process");

                if (userIds == null || userIds.Count == 0)
                {
                    // No specific users given (e.g. a manual full reset) - clear every cached user.
                    await _hybridCache.RemoveByTagAsync(_cacheTagForUsers);
                }
                else
                {
                    // Role/permission change - only reset the users who actually hold the affected
                    // role, instead of every cached user, on both nettime and (via the log below) Gateway.
                    foreach (var userId in userIds)
                    {
                        await _hybridCache.RemoveAsync(_cacheKeyForUsers + userId);
                        await AddTodeletedInMemoryCacheLogs(_cacheKeyForUsers + userId);
                    }
                }

                await _hybridCache.RemoveAsync(_cacheKeyForPermissions);

                // Tell Gateway's in-memory cache to drop its copy of the global permissions list too.
                await AddTodeletedInMemoryCacheLogs(_cacheKeyForPermissions);

                // Step 3: Logging Information: Completed
                _logger.LogInformation("IdentityManager.AuthResetUserCache - Completed");

                return;
            }
            catch (Exception ex)
            {
                // Step 1: Logging Error
                _logger.LogError("IdentityManager.AuthResetUserCache - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Reset User and Permission Cache
        /// </summary>
        public async Task ResetUserCache(List<string> userIds = null)
        {
            await AuthResetUserCache(userIds);
        }

        public async Task ResetIdentityUserCache(string userName_userId_userOid)
        {
            await _hybridCache.RemoveAsync(_cacheKeyForUsers + userName_userId_userOid);
            await AddTodeletedInMemoryCacheLogs(_cacheKeyForUsers + userName_userId_userOid);
            return;
        }

        /// <summary>
        /// Get Permissions
        /// </summary>
        /// <returns>Permission List</returns>
        public async Task<List<NetAuth.Contract.DataContract.Entities.Permission>> GetPermissionsAsync()
        {
            try
            {
                var entryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(_inMemoryPermissionCacheExpiryHours),
                    LocalCacheExpiration = TimeSpan.FromHours(_permissionsCacheExpiryHour)
                };

                return await this._hybridCache.GetOrCreateAsync<List<NetAuth.Contract.DataContract.Entities.Permission>>(_cacheKeyForPermissions,
                  async _ => await GetPermissionsFromNetAuthAsync()
                  , entryOptions);


            }
            catch (Exception ex)
            {
                _logger.LogError("IdentityManager.GetPermissionsAsync - " + ex.Message);
                throw;
            }
        }
        private async Task<List<NetAuth.Contract.DataContract.Entities.Permission>> GetPermissionsFromNetAuthAsync()
        {
            try
            {
                //Step 1: Get permissions from User Service
                List<NetAuth.Contract.DataContract.Entities.Permission> permissionList = await _netAuthUser.GetPermissionsAsync();

                //Step 2: if response is null, then return null
                if (permissionList == null || permissionList.Count == 0)
                {
                    _logger.LogInformation("UserDataAccess.GetPermissions - Permissions NotFound.");
                    return null;
                }

                //Step 3: Logging Information Completed
                _logger.LogInformation("IdentityManager.GetPermissionsAsync - Completed");

                //Step 4: Return permissions
                return permissionList;
            }
            catch (Exception ex)
            {
                _logger.LogError("IdentityManager.GetPermissionsAsync - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get all Permissions, active and inactive
        /// </summary>
        /// <returns>Permission List</returns>
        public async Task<List<NetAuth.Contract.DataContract.Entities.Permission>> GetAllPermissionsAsync()
        {
            try
            {
                var entryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromHours(_inMemoryPermissionCacheExpiryHours),
                    LocalCacheExpiration = TimeSpan.FromHours(_permissionsCacheExpiryHour)
                };

                return await this._hybridCache.GetOrCreateAsync<List<NetAuth.Contract.DataContract.Entities.Permission>>(_cacheKeyForAllPermissions,
                  async _ => await GetAllPermissionsFromNetAuthAsync()
                  , entryOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("IdentityManager.GetAllPermissionsAsync - " + ex.Message);
                throw;
            }
        }

        private async Task<List<NetAuth.Contract.DataContract.Entities.Permission>> GetAllPermissionsFromNetAuthAsync()
        {
            try
            {
                //Step 1: Get all permissions from User Service
                List<NetAuth.Contract.DataContract.Entities.Permission> permissionList = await _netAuthUser.GetAllPermissionsAsync();

                //Step 2: if response is null, then return null
                if (permissionList == null || permissionList.Count == 0)
                {
                    _logger.LogInformation("UserDataAccess.GetAllPermissions - Permissions NotFound.");
                    return null;
                }

                //Step 3: Logging Information Completed
                _logger.LogInformation("IdentityManager.GetAllPermissionsAsync - Completed");

                //Step 4: Return permissions
                return permissionList;
            }
            catch (Exception ex)
            {
                _logger.LogError("IdentityManager.GetAllPermissionsAsync - " + ex.Message);
                throw;
            }
        }

        private async Task<List<DeletedInMemoryCacheLog>> GetdeletedInMemoryCacheLogsAsync()
        {
            var DeletedInMemoryCacheLogs = new List<DeletedInMemoryCacheLog>();

            string query = "SELECT  [CacheKey],[DeletionTimeInUTC] FROM [dbo].[DeletedInMemoryCacheLog]";
            using (SqlDataReader reader = await SqlHelper.ExecuteReaderAsync(this._configuration["ConnectionStrings:SqlDBConnection"], CommandType.Text, query))
            {
                while (await reader.ReadAsync())
                {
                    var log = new DeletedInMemoryCacheLog
                    {
                        CacheKey = reader["CacheKey"].ToString(),
                        DeletionTimeInUTC = reader["DeletionTimeInUTC"] != DBNull.Value ? Convert.ToDateTime(reader["ExpiresAtTime"]) : DateTime.MinValue
                    };
                    DeletedInMemoryCacheLogs.Add(log);
                }
            }

            return DeletedInMemoryCacheLogs;
        }

        private async Task AddTodeletedInMemoryCacheLogs(string cacheKey)
        {
            try
            {
                string insertQuery = "INSERT INTO [dbo].[DeletedInMemoryCacheLog] ([CacheKey], [DeletionTimeInUTC]) VALUES (@CacheKey, @DeletionTimeInUTC)";

                using (SqlConnection connection = new SqlConnection(this._configuration["ConnectionStrings:SqlDBConnection"]))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@CacheKey", cacheKey);
                        command.Parameters.AddWithValue("@DeletionTimeInUTC", DateTime.UtcNow);

                        await command.ExecuteNonQueryAsync();
                    }
                }

            }
            catch (Exception)
            {

            }
        }

        public async Task<List<NetAuth.Contract.DataContract.Dto.UserDto>> GetUserByRoleIdAsync(string roleId)
        {
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.GetUserByRoleIdAsync - In process");

                //Step 2: Call NetAuth api for GetUsers
                List<NetAuth.Contract.DataContract.Dto.UserDto> users = await _netAuthUser.GetUserByRoleId(roleId);


                //Step 3: Logging Information Completed
                _logger.LogInformation("UserDataAccess.GetUserByRoleIdAsync - Completed");

                //Step 4: Return user
                return users;

            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetUserByRoleIdAsync - " + ex.Message);
                throw;
            }
        }

        public async Task<string> CreateUserAsync(string username, string password, string firstName, string lastName, string mobile, string oid, string auth_type)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Username and password are required.");

            if (!Enum.TryParse<Domain.Enums.AuthType>(
           auth_type,
           true,
           out var parsedAuthType))
            {
                throw new ArgumentException("Invalid auth type");
            }

            IdentityUser user = await GetIdentityUserAsync(username);
            if (user != null)
                throw new InvalidOperationException("User already exists.");

            var passwordHash = "";
            if (parsedAuthType == Domain.Enums.AuthType.db)
            {
                passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            }
            //var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var userId = await _netAuthUser.AddUser(new NetAuth.Contract.DataContract.Requests.CreateUserRequest()
            {
                UserName = username,
                preferred_username = username,
                Email = username,
                AccessLevel = Domain.Enums.AccessLevel.L1.ToString(),
                Id = Guid.NewGuid().ToString(),
                oid = oid,
                auth_type = parsedAuthType.ToString(),
                Mobile = mobile,
                FirstName = firstName,
                LastName = lastName,
                display_name = $"{firstName} {lastName}",
                PasswordHash = passwordHash,
            });

            await _hybridCache.RemoveAsync(_cacheKeyForUsers + username);

            return userId;
        }
        public async Task<IdentityUser> ValidateIdentityUserAsync(string username, string password)
        {
            if (username.IsNullOrWhiteSpace()) return null;
            IdentityUser user = await GetIdentityUserAsync(username);
            if (user == null) return null;

            NetAuth.Contract.DataContract.Entities.UserPasswordHash userPasswordHash = await _netAuthUser.GetUserPasswordHash(user.UserId);
            var data = BCrypt.Net.BCrypt.Verify(password, userPasswordHash.PasswordHash) ? user : null;
            return data;
        }
    }


}
