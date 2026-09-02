using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetAuth.Contract.DataContract.Dto;
using NetAuth.Contract.DataContract.Entities;
using netauthlib;

namespace Infrastructure.DataAccess
{
    /// <summary>
    /// Data Access layer :where we write code to connect DB and fetch or manipulate records from DB.
    /// </summary>
    public class UiPermissionDataAccess : IUiPermissionDataAccess
    {
        private ILogger<UiPermissionDataAccess> _logger;
        private IConfiguration _configuration;
        private readonly INetAuthProvider _netAuthUser;
        private readonly IDomainEventService _domainEventService;
        private readonly IMemoryCacheService _inMemoryCacheService;
        private readonly string _cacheKeyForUiPermissions;
        private double _cacheExpiryHours = 30;
        private string _cachePrefix;

        public UiPermissionDataAccess(IWebHostEnvironment env, IConfiguration configuration, ILogger<UiPermissionDataAccess> logger, INetAuthProvider netAuthUser, IDomainEventService domainEventService, IMemoryCacheService inMemoryCacheService = null)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._netAuthUser = netAuthUser;
            this._domainEventService = domainEventService;
            this._inMemoryCacheService = inMemoryCacheService;
            this._cachePrefix = $"{env.EnvironmentName}|{configuration["Api:internal_name"]}|";
            this._cacheKeyForUiPermissions = _cachePrefix + "UiPermissionDataAccess|GetUiPermissions";
        }

        /// <summary>
        /// Get UiPermissions For Role
        /// </summary>
        public async Task<List<RoleUiPermissionDto>> GetUiPermissionsForRole(string roleId)
        {
            try
            {
                _logger.LogInformation("UiPermissionDataAccess.GetUiPermissionsForRole - In process");

                var roleUiPermission = await _netAuthUser.GetUiPermissionsForRole(roleId);

                _logger.LogInformation("UiPermissionDataAccess.GetUiPermissionsForRole - Completed");

                return roleUiPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError("UiPermissionDataAccess.GetUiPermissionsForRole - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get UiPermissions
        /// </summary>
        public async Task<List<UiPermission>> GetUiPermissions()
        {
            try
            {
                _logger.LogInformation("UiPermissionDataAccess.GetUiPermissions - In process");

                Object cachedPermissionsObj = await _inMemoryCacheService.GetCacheValueAsync(_cacheKeyForUiPermissions);
                List<UiPermission> cachedPermissions = (List<UiPermission>)cachedPermissionsObj;
                if (cachedPermissions != null && cachedPermissions.Count > 0)
                {
                    _logger.LogInformation("Returned from CacheKey: " + _cacheKeyForUiPermissions);
                    return cachedPermissions;
                }

                var allUiPermissionsList = await _netAuthUser.GetUiPermissions();

                await _inMemoryCacheService.SetCacheValueAsync(_cacheKeyForUiPermissions, allUiPermissionsList, TimeSpan.FromHours(_cacheExpiryHours));

                _logger.LogInformation("UiPermissionDataAccess.GetUiPermissions - Completed");

                return allUiPermissionsList;
            }
            catch (Exception ex)
            {
                _logger.LogError("UiPermissionDataAccess.GetUiPermissions - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Add UiPermissions For Role
        /// </summary>
        public async Task<bool> AddUiPermissionsForRole(NetAuth.Contract.DataContract.Requests.AddUiPermissionsForRole addUiPermissionForRole)
        {
            try
            {
                _logger.LogInformation("UiPermissionDataAccess.AddUiPermissionsForRole - In process");

                bool response = await _netAuthUser.AddUiPermissionsForRole(addUiPermissionForRole);

                _logger.LogInformation("UiPermissionDataAccess.AddUiPermissionsForRole - Completed");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("UiPermissionDataAccess.AddUiPermissionsForRole - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Add UiPermission
        /// </summary>
        public async Task<string> AddUiPermission(NetAuth.Contract.DataContract.Requests.AddUiPermission addUiPermission)
        {
            string insertedId = string.Empty;
            try
            {
                _logger.LogInformation("UiPermissionDataAccess.AddUiPermission - In process");

                insertedId = await _netAuthUser.AddUiPermission(addUiPermission);

                _logger.LogInformation("UiPermissionDataAccess.AddUiPermission - Completed");

                return insertedId;
            }
            catch (Exception ex)
            {
                _logger.LogError("UiPermissionDataAccess.AddUiPermission - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Update UiPermission
        /// </summary>
        public async Task<bool> UpdateUiPermission(NetAuth.Contract.DataContract.Requests.UpdateUiPermission updateUiPermission)
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("UiPermissionDataAccess.UpdateUiPermission - In process");

                retval = await _netAuthUser.UpdateUiPermission(updateUiPermission);

                _logger.LogInformation("UiPermissionDataAccess.UpdateUiPermission - Completed");

                return retval;
            }
            catch (Exception ex)
            {
                _logger.LogError("UiPermissionDataAccess.UpdateUiPermission - " + ex.Message);
                throw;
            }
        }

        private async Task DispatchEvents(Domain.Entities.User entity)
        {
            while (true)
            {
                var domainEventEntity = entity.DomainEvents
                    .Where(domainEvent => !domainEvent.IsPublished)
                    .FirstOrDefault();
                if (domainEventEntity == null) break;

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity);
            }
        }

        /// <summary>
        /// Reset UiPermission in UiPermission DataAccess
        /// </summary>
        public async Task ResetUiPermissionCache()
        {
            try
            {
                _logger.LogInformation("UiPermissionDataAccess.ResetUiPermissionCache - In process");

                var cacheKeys = new[] { _cacheKeyForUiPermissions };

                foreach (var key in cacheKeys)
                {
                    await _inMemoryCacheService.RemoveCacheValue(key);
                }

                _logger.LogInformation("UiPermissionDataAccess.ResetUiPermissionCache - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UiPermissionDataAccess.ResetUiPermissionCache - " + ex.Message);
                throw;
            }
        }
    }
}
