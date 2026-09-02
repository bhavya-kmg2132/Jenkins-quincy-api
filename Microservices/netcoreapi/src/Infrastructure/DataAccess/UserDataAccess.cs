using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.SystemManager.UpdateActionPermissionEndPoint;
using AutoMapper;
using Dapper;
using DataAccess.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetAuth.Contract.DataContract.Dto;
using netauthlib;
using Npgsql;

namespace Infrastructure.DataAccess
{
    /// <summary>
    /// Data Access layer :where we write code to connect DB and fetch or manipulate records from DB.
    /// In the database layer, we'll find things like database, connection, table, SQL, and result set.
    /// </summary>
    public class UserDataAccess : IUserDataAccess
    {
        private ILogger<UserDataAccess> _logger;
        private IConfiguration _configuration;
        private readonly INetAuthProvider _netAuthProvider;
        private readonly IMapper _mapper;
        private double _cacheExpiryHours = 8;
        private const string _cacheKeyForUsersFullName = "netcoreapi|UserDataAccess|GetUsersFullName";
        private readonly IMemoryCacheService _memoryCacheService;
        private readonly IConnectionHelper _connectionHelper;


        /// <summary>
        /// Instantiation of UserDataAccess class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public UserDataAccess(IConfiguration configuration, IMapper mapper, ILogger<UserDataAccess> logger, INetAuthProvider netAuthUser, IConnectionHelper connectionHelper, IMemoryCacheService memoryCacheService = null)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._mapper = mapper;
            this._netAuthProvider = netAuthUser;
            this._memoryCacheService = memoryCacheService;
            this._connectionHelper = connectionHelper;
        }

        /// <summary>
        /// Gets User from cache or DB
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserDto> GetUserFromNetAuthLibAsync(string username)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetUserFromNetAuthLibAsync - In process");

                var user = await _netAuthProvider.GetUserVmByUserName(username);

                _logger.LogInformation("UserDataAccess.GetUserFromNetAuthLibAsync - Completed");

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetUserFromNetAuthLibAsync - " + ex.Message);
                throw;
            }
        }

        #region Public methods
        /// <summary>
        /// Add Users
        /// </summary>
        /// <returns>string</returns>
        public async Task<string> AddUser(NetAuth.Contract.DataContract.Requests.CreateUserRequest request)
        {
            string insertedId;
            try
            {
                _logger.LogInformation("UserDataAccess.AddUser - InProcess");

                insertedId = await _netAuthProvider.AddUser(request);

                _logger.LogInformation("UserDataAccess.AddUser - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddUser - " + ex.Message);
                throw;
            }
            return insertedId;
        }
        #endregion

        /// <summary>
        /// Get Permissions Granted For a Role
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<RoleDto> GetPermissionsForRoleAsync(string roleId)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetPermissionsForRoleAsync - In process");

                var role = await _netAuthProvider.GetPermissionsByRoleId(roleId);

                _logger.LogInformation("UserDataAccess.GetPermissionsForRoleAsync - Completed");

                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetPermissionsForRoleAsync - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoleDto>> GetRoles()
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetRoles - In process");

                var roles = await _netAuthProvider.GetRoles();

                _logger.LogInformation("UserDataAccess.GetRoles - Completed");

                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetRoles - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get role by id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<RoleDto> GetRoleById(string roleId)
        {
            if (roleId == "0" || string.IsNullOrEmpty(roleId) || string.IsNullOrWhiteSpace(roleId))
            {
                return null;
            }

            try
            {
                _logger.LogInformation("UserDataAccess.GetRoleById - In process");

                await Task.CompletedTask;

                _logger.LogInformation("UserDataAccess.GetRoleById - Completed");

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetRoleById - " + ex.Message);
                throw;
            }
        }

        public async Task<string> GetUserIdBasedOnOidFromDb(string oid)
        {
            try
            {
                string UserId = string.Empty;
                await Task.CompletedTask;
                return UserId;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetUserIdBasedOnOidFromDb - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets User from DB
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserDto> GetUserFromDb(string userId)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetUserFromDb - In process");

                var user = await _netAuthProvider.GetUserFromDb(userId);

                _logger.LogInformation("UserDataAccess.GetUserFromDb - Completed");

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetUserFromDb - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets User from cache or DB
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserDto> GetUserFromDbAsync(string userId)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetUserFromDbAsync - In process");

                var user = await _netAuthProvider.GetUserFromDb(userId);

                _logger.LogInformation("UserDataAccess.GetUserFromDbAsync - Completed");

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetUserFromDbAsync - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Add Permissions denied For User
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionIds"></param>
        /// <returns>bool</returns>
        public async Task<bool> AddPermissionsDeniedForUser(string userId, List<string> permissionIds, string createdBy)
        {
            bool response = false;
            try
            {
                _logger.LogInformation("UserDataAccess.AddPermissionsDeniedForUser - In process");

                response = await _netAuthProvider.AddPermissionsDeniedForUser(userId, permissionIds, createdBy);

                _logger.LogInformation("UserDataAccess.AddPermissionsDeniedForUser - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddPermissionsDeniedForUser - " + ex.Message);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Add Permissions granted For a User
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionIds"></param>
        /// <returns>bool</returns>
        public async Task<bool> AddPermissionsGrantedForUser(string userId, List<string> permissionIds, string createdBy)
        {
            bool response = false;
            try
            {
                _logger.LogInformation("UserDataAccess.AddPermissionGrantedForUser - In process");

                response = await _netAuthProvider.AddPermissionsGrantedForUser(userId, permissionIds, createdBy);

                await DispatchEvents(new Domain.Entities.User());

                _logger.LogInformation("UserDataAccess.AddPermissionGrantedForUser - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddPermissionGrantedForUser - " + ex.Message);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Add permissions for role
        /// </summary>
        /// <returns>bool</returns>
        public async Task<bool> AddPermissionsForRole(string roleId, List<string> permissionIds, string createdBy)
        {
            bool response = false;
            try
            {
                _logger.LogInformation("UserDataAccess.AddPermissionForRole - In process");

                response = await _netAuthProvider.AddPermissionsForRole(roleId, permissionIds, createdBy);

                await DispatchEvents(new Domain.Entities.User());

                _logger.LogInformation("UserDataAccess.AddPermissionForRole - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddPermissionForRole - " + ex.Message);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Add Roles For User
        /// </summary>
        /// <returns>bool</returns>
        public async Task<bool> AddRolesForUser(string userId, List<string> roleIds, string createdBy)
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("UserDataAccess.AddRolesForUser - In process");

                retval = await _netAuthProvider.AddRoles(userId, roleIds, createdBy);

                _logger.LogInformation("UserDataAccess.AddRolesForUser - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddRolesForUser - " + ex.Message);
                throw;
            }

            return retval;
        }

        /// <summary>
        /// Add Role For User
        /// </summary>
        /// <returns>bool</returns>
        public async Task<bool> AddRoleForUser(string userId, string roleId, string createdBy)
        {
            bool response = false;
            try
            {
                _logger.LogInformation("UserDataAccess.AddRoleForUser - In process");

                response = await _netAuthProvider.AddRole(userId, roleId, createdBy);

                _logger.LogInformation("UserDataAccess.AddRoleForUser - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddRoleForUser - " + ex.Message);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Delete Role For User
        /// </summary>
        /// <returns>bool</returns>
        public async Task<bool> DeleteRoleForUser(string userId, string roleId, string updatedBy)
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("UserDataAccess.DeleteRoleForUser - In process");

                retval = await _netAuthProvider.DeleteRole(userId, roleId, updatedBy);

                _logger.LogInformation("UserDataAccess.DeleteRoleForUser - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.DeleteRoleForUser - " + ex.Message);
                throw;
            }

            return retval;
        }

        /// <summary>
        /// Updates access level of user in user table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserAccessLevel(UserDto user)
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("UserDataAccess.UpdateUserAccessLevel - In process");

                await Task.CompletedTask;

                _logger.LogInformation("UserDataAccess.UpdateUserAccessLevel - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.UpdateUserAccessLevel - " + ex.Message);
                throw;
            }

            return retval;
        }

        /// <summary>
        ///  Get UserAccessLevel List
        /// </summary>
        /// <returns>Access Levels</returns>
        public async Task<List<NetAuth.Contract.DataContract.Entities.UserAccessLevel>> GetUserAccessLevelList()
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetUserAccessLevelList - In process");

                await Task.CompletedTask;

                return new List<NetAuth.Contract.DataContract.Entities.UserAccessLevel>();
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetUserAccessLevelList - " + ex.Message);
                throw;
            }
        }


        /// <summary>
        ///  Gets User Profile
        /// </summary>
        /// <returns>User Profile</returns>
        public async Task<List<NetAuth.Contract.DataContract.Entities.UserProfile>> GetUserProfileByUserId(string userId)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetUserProfileByUserId - In process");

                await Task.CompletedTask;
                return new List<NetAuth.Contract.DataContract.Entities.UserProfile>();
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetUserProfileByUserId - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        ///  Gets User Profile
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns>User Profile</returns>
        public async Task<List<NetAuth.Contract.DataContract.Entities.UserProfile>> GetUserProfileByProfileId(int profileId)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetUserProfileByProfileId - In process");

                await Task.CompletedTask;

                return new List<NetAuth.Contract.DataContract.Entities.UserProfile>();
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetUserProfileByProfileId - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        ///  Gets list of User Profiles
        /// </summary>
        /// <returns>User Profile</returns>
        public async Task<List<NetAuth.Contract.DataContract.Entities.UserProfile>> GetUserProfileList()
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetUserProfileList - In process");

                await Task.CompletedTask;

                return new List<NetAuth.Contract.DataContract.Entities.UserProfile>();
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetUserProfileList - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get AuthReferenceLookupList
        /// </summary>
        /// <returns></returns>
        public async Task<List<NetAuth.Contract.DataContract.Entities.AuthReferenceLookup>> GetAuthReferenceLookupList(string type)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetAuthReferenceLookupList - In process");

                var lookupList = await _netAuthProvider.GetAuthReferenceLookupList(type);

                _logger.LogInformation("UserDataAccess.GetAuthReferenceLookupList - Completed");

                return lookupList;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetAuthReferenceLookupList - " + ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Add permissions
        /// </summary>
        /// <returns>bool</returns>
        public async Task<bool> AddPermissions(string permissionValue, string permissionDisplayName, string userName)
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("UserDataAccess.AddPermissions - In process");

                await DispatchEvents(new Domain.Entities.User());

                _logger.LogInformation("UserDataAccess.AddPermissions - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddPermissions - " + ex.Message);
                throw;
            }

            return retval;
        }

        /// <summary>
        /// Add permission
        /// </summary>
        /// <returns>bool</returns>
        public async Task<bool> AddPermission(NetAuth.Contract.DataContract.Requests.AddPermission permission, string userName)
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("UserDataAccess.AddPermission - In process");

                retval = await _netAuthProvider.AddPermission(permission, userName);

                await DispatchEvents(new Domain.Entities.User());

                _logger.LogInformation("UserDataAccess.AddPermission - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddPermission - " + ex.Message);
                throw;
            }

            return retval;
        }

        /// <summary>
        /// Update permission
        /// </summary>
        /// <returns>bool</returns>
        public async Task<bool> UpdatePermission(NetAuth.Contract.DataContract.Requests.UpdatePermission permission, string userName)
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("UserDataAccess.UpdatePermission - In process");

                retval = await _netAuthProvider.UpdatePermission(permission, userName);

                await DispatchEvents(new Domain.Entities.User());

                _logger.LogInformation("UserDataAccess.UpdatePermission - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.UpdatePermission - " + ex.Message);
                throw;
            }

            return retval;
        }

        /// <summary>
        /// Log event and publish
        /// </summary>
        private async Task DispatchEvents(Domain.Entities.User entity)
        {
            while (true)
            {
                var domainEventEntity = entity.DomainEvents
                    .Where(domainEvent => !domainEvent.IsPublished)
                    .FirstOrDefault();
                if (domainEventEntity == null) break;

                domainEventEntity.IsPublished = true;
            }
            await Task.CompletedTask;
        }

        public DataSet UserListReport()
        {
            try
            {
                _logger.LogInformation("MasterDataAccess.UserListReport - In process");

                _logger.LogInformation("MasterDataAccess.UserListReport - Completed");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("MasterDataAccess.UserListReport - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get User UiPermissions By UserId(Role+Granted+Denied)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<UserUiPermissionDto>> GetUserUiPermissionsByUserId(string userId)
        {
            try
            {
                var user = await GetUserFromDbAsync(userId);
                return user?.UserUiPermissions ?? new List<UserUiPermissionDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserDataAccess.GetUserUiPermissionsByUserId - Error while fetching UI Permission for ID: {userId}");
                throw;
            }
        }

        public async Task<NetAuth.Contract.DataContract.Entities.UserPasswordHash> GetUserPasswordHash(string userId)
        {
            return await _netAuthProvider.GetUserPasswordHash(userId);
        }

        #region UserActivity

        /// <summary>
        /// Add UserActivity
        /// </summary>
        /// <param name="userActivity"></param>
        /// <returns></returns>
        public async Task<string> AddUserActivity(NetAuth.Contract.DataContract.Requests.AddUserActivity userActivity)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.AddUserActivity - In process");

                string userActivityId = await _netAuthProvider.AddUserActivity(userActivity);

                await DispatchEvents(new Domain.Entities.User());

                _logger.LogInformation("UserDataAccess.AddUserActivity - Completed");

                return userActivityId;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddUserActivity - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get UserActivities
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public async Task<List<UserActivityDto>> GetUserActivities(string userId, int pageSize, int pageNumber, int period)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetUserActivities - In process");

                string startDate = DateTime.UtcNow.AddDays(-period).ToString("yyyy-MM-dd");
                string endDate   = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd");

                var userActivityList = await _netAuthProvider.GetUserActivities(userId, pageSize, pageNumber, startDate, endDate);

                _logger.LogInformation("UserDataAccess.GetUserActivities - Completed");

                return userActivityList;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetUserActivities - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get UserActivities By UserIds
        /// </summary>
        /// <returns></returns>
        public async Task<List<NetAuth.Contract.DataContract.Entities.UserActivity>> GetUserActivitiesByUserIds(List<string> userIds)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetUserActivitiesByUserIds - In process");

                var userActivityList = await _netAuthProvider.GetUserActivitiesByUserIds(userIds);

                _logger.LogInformation("UserDataAccess.GetUserActivitiesByUserIds - Completed");

                return userActivityList;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetUserActivitiesByUserIds - " + ex.Message);
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Update UserPasswordHash
        /// </summary>
        /// <returns>int</returns>
        public async Task<int> UpdateUserPasswordHash(NetAuth.Contract.DataContract.Requests.UpdateUserPasswordHash updateUserPasswordHash)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.UpdateUserPasswordHash - In process");

                int recordId = await _netAuthProvider.UpdateUserPasswordHash(updateUserPasswordHash);

                await DispatchEvents(new Domain.Entities.User());

                _logger.LogInformation("UserDataAccess.UpdateUserPasswordHash - Completed");

                return recordId;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.UpdateUserPasswordHash - " + ex.Message);
                throw;
            }
        }

        public async Task<int> UpdateUser(NetAuth.Contract.DataContract.Requests.UpdateUser updateUser)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.UpdateUser - In process");

                int recordId = await _netAuthProvider.UpdateUser(updateUser);

                await DispatchEvents(new Domain.Entities.User());

                _logger.LogInformation("UserDataAccess.UpdateUser - Completed");

                return recordId;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.UpdateUser - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// ActivateOrInActivateUser
        /// </summary>
        /// <returns></returns>
        public async Task<int> ActivateOrInActivateUser(NetAuth.Contract.DataContract.Requests.ActivateOrInActivateUser activateOrInActivateUser)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.ActivateOrInActivateUser - In process");

                int userActivityId = await _netAuthProvider.ActivateOrInActivateUser(activateOrInActivateUser);

                await DispatchEvents(new Domain.Entities.User());

                _logger.LogInformation("UserDataAccess.ActivateOrInActivateUser - Completed");

                return userActivityId;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.ActivateOrInActivateUser - " + ex.Message);
                throw;
            }
        }

        public async Task<List<Domain.Entities.UserName>> GetUserFullName()
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetUserFullName - In process");

                List<Domain.Entities.UserName> users = (List<Domain.Entities.UserName>)await _memoryCacheService.GetCacheValueAsync(_cacheKeyForUsersFullName);

                if (users != null)
                {
                    _logger.LogInformation("UserDataAccess.GetUserFullName - Data retrieved from cache.");
                    return users;
                }

                _logger.LogInformation("UserDataAccess.GetUserFullName - Cache miss, fetching from database.");
                users = new List<Domain.Entities.UserName>();

                var query = @"
                SELECT
                    u.""Id"" AS Id,
                    u.""Email"" AS Email,
                    u.""IsActive"" AS IsActive,
                    u.""display_name"" AS FullName
                FROM ""User"" AS u;;";
                using (var connection = new NpgsqlConnection(_configuration["NetAuth.ConnectionStrings:PostgreSqlDBConnection"]))
                {
                    users = (await connection.QueryAsync<Domain.Entities.UserName>(query)).ToList();
                }
                await _memoryCacheService.SetCacheValueAsync(_cacheKeyForUsersFullName, users, TimeSpan.FromHours(_cacheExpiryHours));

                _logger.LogInformation("UserDataAccess.GetUserFullName - Completed");

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError($"UserDataAccess.GetUserFullName - Error: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<List<UsersDto>> GetUsersByStatus(string status)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetUsersByStatus - In process");

                var users = await _netAuthProvider.GetUsersByStatus(status);

                _logger.LogInformation("UserDataAccess.GetUsersByStatus - Completed");
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetUsersByStatus - " + ex.Message);
                throw;
            }
        }

        #region Team management

        public async Task<List<TeamDto>> GetTeams()
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetTeams - In process");
                var teams = await _netAuthProvider.GetTeams();
                _logger.LogInformation("UserDataAccess.GetTeams - Completed");
                return teams;
            }
            catch (Exception ex) { _logger.LogError("UserDataAccess.GetTeams - " + ex.Message); throw; }
        }

        public async Task<TeamDto> GetTeamById(string teamId)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetTeamById - In process");
                var team = await _netAuthProvider.GetTeamById(teamId);
                _logger.LogInformation("UserDataAccess.GetTeamById - Completed");
                return team;
            }
            catch (Exception ex) { _logger.LogError("UserDataAccess.GetTeamById - " + ex.Message); throw; }
        }

        public async Task<string> AddTeam(TeamDto team, string createdBy)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.AddTeam - In process");
                var newId = await _netAuthProvider.AddTeam(team, createdBy);
                _logger.LogInformation("UserDataAccess.AddTeam - Completed");
                return newId;
            }
            catch (Exception ex) { _logger.LogError("UserDataAccess.AddTeam - " + ex.Message); throw; }
        }

        public async Task<bool> AddTeamMembers(string teamId, List<string> userIds, string createdBy)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.AddTeamMembers - In process");
                var result = await _netAuthProvider.AddTeamMembers(teamId, userIds, createdBy);
                _logger.LogInformation("UserDataAccess.AddTeamMembers - Completed");
                return result;
            }
            catch (Exception ex) { _logger.LogError("UserDataAccess.AddTeamMembers - " + ex.Message); throw; }
        }

        public async Task<bool> RemoveTeamMember(string teamId, string memberId)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.RemoveTeamMember - In process");
                var result = await _netAuthProvider.RemoveTeamMember(teamId, memberId);
                _logger.LogInformation("UserDataAccess.RemoveTeamMember - Completed");
                return result;
            }
            catch (Exception ex) { _logger.LogError("UserDataAccess.RemoveTeamMember - " + ex.Message); throw; }
        }

       
        public async Task<List<TeamDto>> GetTeamsByUserId(string userId)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetTeamsByUserId - In process");
                var teams = await _netAuthProvider.GetTeamsByUserId(userId);
                _logger.LogInformation("UserDataAccess.GetTeamsByUserId - Completed");
                return teams;
            }
            catch (Exception ex) { _logger.LogError("UserDataAccess.GetTeamsByUserId - " + ex.Message); throw; }
        }

       

        public async Task<List<TeamMemberDto>> GetTeamMembersByTeamId(string teamId)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetTeamMembersByTeamId - In process");
                var r = await _netAuthProvider.GetTeamMembersByTeamId(teamId);
                _logger.LogInformation("UserDataAccess.GetTeamMembersByTeamId - Completed");
                return r;
            }
            catch (Exception ex) { _logger.LogError("UserDataAccess.GetTeamMembersByTeamId - " + ex.Message); throw; }
        }

        #endregion

        public async Task AddActionPermissionEndPoint(List<UpdateActionPermissionEndPointDto> items, string moduleId, string permissionSetId, string permissionType, string apiName, string createdBy)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.AddActionPermissionEndPoint - In process");

                var query = _connectionHelper.LoadSqlQueriesXml("Auth")["CodePermission.Add"];

                using var connection = _connectionHelper.CreateNetAuthConnection();

                foreach (var item in items)
                {
                    await connection.ExecuteAsync(query, new
                    {
                        item.PermissionValue,
                        item.PermissionDisplayName,
                        ModuleId = moduleId,
                        PermissionSetId = permissionSetId,
                        PermissionType = permissionType,
                        ApiName = apiName,
                        item.ActionPermissionEndPoint,
                        CreatedBy = createdBy
                    });
                }

                _logger.LogInformation("UserDataAccess.AddActionPermissionEndPoint - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddActionPermissionEndPoint - " + ex.Message);
                throw;
            }
        }

        public async Task<List<string>> UpdateCodePermissionActionEndpoints(List<UpdateActionPermissionEndPointDto> items, string updatedBy)
        {
            var updatedPermissionValues = new List<string>();
            try
            {
                _logger.LogInformation("UserDataAccess.UpdateCodePermissionActionEndpoints - In process");

                var query = _connectionHelper.LoadSqlQueriesXml("Auth")["CodePermission.UpdateActionEndpoint"];

                using var connection = _connectionHelper.CreateNetAuthConnection();

                foreach (var item in items)
                {
                    var rowsAffected = await connection.ExecuteAsync(query, new
                    {
                        item.PermissionValue,
                        item.ActionPermissionEndPoint,
                        UpdatedBy = updatedBy
                    });

                    if (rowsAffected > 0)
                    {
                        updatedPermissionValues.Add(item.PermissionValue);
                    }
                }

                _logger.LogInformation("UserDataAccess.UpdateCodePermissionActionEndpoints - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.UpdateCodePermissionActionEndpoints - " + ex.Message);
                throw;
            }

            return updatedPermissionValues;
        }
    }
}
