using System.Data;
using System.Net;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetAuth.Application.Common.Interfaces;
using NetAuth.Domain.Dto;
using NetAuth.Domain.Entities;
using NetAuth.Domain.Entities.CoreRequests;
using NetAuth.Interfaces;
using NetAuth.Lib.Domain.Entities.Entities;

namespace NetAuth.DataAccess
{
    /// <summary>
    /// Data Access layer :where we write code to connect DB and fetch or manipulate records from DB.
    /// In the database layer, we'll find things like database, connection, table, SQL, and result set.
    /// </summary>
    internal class UserDataAccess : IUserDataAccess
    {
        private ILogger<UserDataAccess> _logger;
        private IConfiguration _configuration;
        private IUserLoader _userLoader;
        private readonly IConnectionHelper _connectionHelper;

        private IIdentityManager _identityManager;
        private readonly Dictionary<string, string> _sqlQueries;

        /// <summary>
        /// Instantiation of UserDataAccess class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public UserDataAccess(IConfiguration configuration, ILogger<UserDataAccess> logger, IUserLoader userLoader, IIdentityManager identityManager, IConnectionHelper connectionHelper)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._userLoader = userLoader;
            this._identityManager = identityManager;
            this._connectionHelper = connectionHelper;
            this._sqlQueries = _connectionHelper.LoadSqlQueriesXml("Auth");
        }


        #region Public methods 
        /// <summary>
        /// Add Users
        /// </summary>
        /// <param name="prospect"></param>
        /// <param name="constr"></param>
        /// <returns>string</returns>
        public async Task<string> AddUser(CreateUserRequest user)
        {
            string insertedId;
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.AddUser - In process");

                if (string.IsNullOrEmpty(user.UserName))
                {
                    throw new ApplicationException("UserName can not be null or empty !");
                }

                if (string.IsNullOrEmpty(user.auth_type))
                {
                    throw new ApplicationException("auth_type can not be null or empty !");
                }

                if (string.IsNullOrEmpty(user.Mobile))
                {
                    throw new ApplicationException("Mobile can not be null or empty !");
                }

                if (string.IsNullOrEmpty(user.Email))
                {
                    throw new ApplicationException("Email can not be null or empty !");
                }

                if (string.IsNullOrEmpty(user.FirstName))
                {
                    throw new ApplicationException("FirstName can not be null or empty !");
                }

                if (string.IsNullOrEmpty(user.LastName))
                {
                    throw new ApplicationException("LastName can not be null or empty !");
                }

                if (string.IsNullOrEmpty(user.display_name))
                {
                    throw new ApplicationException("display_name can not be null or empty !");
                }


                var parameters = new
                {
                    Id = user.Id ?? Guid.NewGuid().ToString(),
                    EmpId = user.EmpId,
                    EmpType = user.EmpType,
                    PasswordHash = user.PasswordHash,
                    auth_type = user.auth_type,
                    UserName = user.UserName,
                    mobile = user.Mobile,
                    Email = user.Email,
                    Position = user.Position,
                    BusinessUnit = user.BusinessUnit,
                    oid = user.oid,
                    given_name = user.given_name,
                    family_name = user.family_name,
                    preferred_username = user.preferred_username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    SecondaryEmail = user.SecondaryEmail,
                    PhoneNumber = user.PhoneNumber,
                    Extension = user.Extension,
                    display_name = user.display_name,
                    ManagerId = user.ManagerId,
                    AccessLevel = user.AccessLevel,
                    Designation = user.Designation,
                    Department = user.Department,
                    Location = user.Location,
                    Organization = user.Organization,

                    // Auditable Fields
                    IsAuthorized = (bool?)null,
                    OwnerId = (string?)null,
                    SysData = (string?)null,
                    TenantId = (string?)null,
                    SubTenantId = (string?)null,
                    CreatedBy = user.CreatedBy
                };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    //Step 4 : Execute Scalar
                    insertedId = await _dapperDbConnection.ExecuteScalarAsync<string>(_sqlQueries["User.Save"], parameters);
                }

                //Step 5: Logging Information - Completed
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
                IEnumerable<Permission> rolePermission = Enumerable.Empty<Permission>();

                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.PermissionsForRoleAsync - In process");

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    //Step 2: Assigning values to Sql Parameters
                    rolePermission = await _dapperDbConnection.QueryAsync<NetAuth.Domain.Entities.Permission>(_sqlQueries["RolePermissionInfo.Select"], new { RoleId = roleId });
                }

                var role = new RoleDto();
                role.Id = roleId;

                role.RolePermissions = rolePermission.ToList();


                _logger.LogInformation("UserDataAccess.PermissionsForRoleAsync - Completed");
                //Step 5 : Return permissions
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.PermissionsForRoleAsync - " + ex.Message);
                throw;
            }
        }

        public async Task<List<RoleDto>> GetRoles()
        {
            try
            {
                List<RoleDto> roleList = new List<RoleDto>();
                List<RolePermission> permissionsList = new List<RolePermission>();

                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.GetRoles - In process");

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    //Step 2: Execute
                    using var multi = await _dapperDbConnection.QueryMultipleAsync(_sqlQueries["RoleInfo.Select"], new { RoleId = "0" });

                    roleList = (await multi.ReadAsync<RoleDto>()).ToList();
                    permissionsList = (await multi.ReadAsync<RolePermission>()).ToList();
                }
                List<RoleDto> roles = new List<RoleDto>();

                roles.AddRange(roleList);

                foreach (var rolePermission in permissionsList)
                {
                    var role = roles.Find(u => u.Id.Equals(rolePermission.Id));
                    role.RolePermissions = new List<NetAuth.Domain.Entities.Permission>();

                    var permission = new NetAuth.Domain.Entities.Permission();
                    permission.PermissionId = rolePermission.PermissionId;
                    permission.PermissionValue = rolePermission.PermissionValue;
                    permission.PermissionDisplayName = rolePermission.PermissionDisplayName;
                    permission.PermissionSetId = rolePermission.PermissionSetId;
                    permission.PermissionSetName = rolePermission.PermissionSetName;
                    permission.ModuleId = rolePermission.ModuleId;
                    permission.ModuleName = rolePermission.ModuleName;
                    permission.ActionPermissionEndPoint = rolePermission.ActionPermissionEndPoint;
                    permission.IsActive = rolePermission.IsActive;

                    role.RolePermissions.Add(permission);
                }

                //Step 3: Logging Information Completed
                _logger.LogInformation("UserDataAccess.GetRoles - Completed");

                //Step 4 : Return roles
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetRoles - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List<NetAuth.Domain.Entities.User></returns>
        public async Task<RoleDto> GetRoleById(string roleId)
        {
            if (roleId == "0" || string.IsNullOrEmpty(roleId) || string.IsNullOrWhiteSpace(roleId))
            {
                return null;
            }

            try
            {
                RoleDto role = new RoleDto();
                List<RolePermission> permissionsList = new List<RolePermission>();

                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.GetRoleById - In process");

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    //Step 2: Execute
                    using var multi = await _dapperDbConnection.QueryMultipleAsync(_sqlQueries["RoleInfo.Select"], new { RoleId = "0" });

                    role = (await multi.ReadAsync<RoleDto>()).FirstOrDefault();
                    permissionsList = (await multi.ReadAsync<RolePermission>()).ToList();
                }

                role.RolePermissions = new List<NetAuth.Domain.Entities.Permission>();


                foreach (var rolePermission in permissionsList)
                {
                    var permission = new NetAuth.Domain.Entities.Permission();
                    permission.PermissionId = rolePermission.PermissionId;
                    permission.PermissionValue = rolePermission.PermissionValue;
                    permission.PermissionDisplayName = rolePermission.PermissionDisplayName;
                    permission.PermissionSetId = rolePermission.PermissionSetId;
                    permission.PermissionSetName = rolePermission.PermissionSetName;
                    permission.ModuleId = rolePermission.ModuleId;
                    permission.ModuleName = rolePermission.ModuleName;
                    permission.ActionPermissionEndPoint = rolePermission.ActionPermissionEndPoint;
                    permission.IsActive = rolePermission.IsActive;

                    role.RolePermissions.Add(permission);
                }

                //Step 5: Logging Information Completed
                _logger.LogInformation("UserDataAccess.GetRoleById - Completed");

                //Step 6 : Return roles
                return role;
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
                var userId = string.Empty;

                _logger.LogInformation("UserDataAccess.GetUserIdBasedOnOidFromDb - In process");

                if (string.IsNullOrWhiteSpace(oid))
                    return string.Empty;

                // string query = @"SELECT id FROM ""User"" WHERE oid = @Oid LIMIT 1;";
                string query = _sqlQueries["User.GetIdByOid"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {

                    userId = await _dapperDbConnection.QueryFirstOrDefaultAsync<string>(query, new { Oid = oid });

                }
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError($"UserDataAccess.GetUserIdBasedOnOidFromDb - Empty userId returned for oid={oid}");
                    return string.Empty;
                }

                _logger.LogInformation("UserDataAccess.GetUserIdBasedOnOidFromDb - Completed");

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"UserDataAccess.GetUserIdBasedOnOidFromDb - {ex.Message}");
                throw;
            }
        }

        public async Task<UserDto> GetUserFromDbAsync(string userId)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetUserFromDbAsync - In process");

                UserDto user;
                List<RoleDto> roleList;
                List<Permission> permissionsGranted;
                List<Permission> permissionsDenied;
                List<IdentityUserUiPermission> uiPermissionsList;
                List<IdentityUserTeam> userTeamList;

                // Use a local connection so GetPermissionsForRoleAsync (which uses _dapperDbConnection)
                // does not conflict with the open GridReader.
                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    using var multi = await _dapperDbConnection.QueryMultipleAsync(_sqlQueries["UserInfo.Select"], new { userId = userId });

                    // Result set 1: user info
                    user = (await multi.ReadAsync<UserDto>()).FirstOrDefault();

                    if (user == null)
                    {
                        _logger.LogInformation($"UserDataAccess.GetUserFromDbAsync - No user found for {userId}");
                        return null;
                    }

                    roleList = (await multi.ReadAsync<RoleDto>()).ToList();
                    permissionsGranted = (await multi.ReadAsync<Permission>()).ToList();
                    permissionsDenied = (await multi.ReadAsync<Permission>()).ToList();
                    // Result set 5: UI permissions (Role + UserGranted + UserDenied + Team — merged and deduplicated by SQL)
                    uiPermissionsList = (await multi.ReadAsync<IdentityUserUiPermission>()).ToList();
                    // Result set 6: User Teams; Result set 7: Team Permissions — must match SQL order
                    userTeamList = (await multi.ReadAsync<IdentityUserTeam>()).ToList();

                }

                // Populate roles
                user.Roles = roleList;
                foreach (var role in user.Roles)
                    role.RolePermissions = new List<Permission>();

                // Build UserPermissions: role permissions first
                user.UserPermissions = new List<Permission>();
                await AddRolePermissionsForUser(user);

                // Teams
                user.Teams = userTeamList
                    .Select(t => new TeamDto { Id = t.Id, TeamName = t.TeamName, TeamShortName = t.TeamShortName, Description = t.Description, TeamOwnerId = t.TeamOwnerId, TeamCaptainId = t.TeamCaptainId })
                    .ToList();


                // Add UserGranted and team API permissions
                user.PermissionsGranted = permissionsGranted;
                user.UserPermissions.AddRange(permissionsGranted);

                // Remove denied permissions, then deduplicate
                user.PermissionsDenied = permissionsDenied;
                user.UserPermissions = user.UserPermissions
                    .Where(p => !permissionsDenied.Any(d => d.PermissionId == p.PermissionId))
                    .ToList();

                FilterDuplicatePermission(user);

                // UI permissions (already merged in SQL) — map
                AddUiPermissionsForUser(user, uiPermissionsList);

                FilterDuplicatedUiPermission(user);

                _logger.LogInformation("UserDataAccess.GetUserFromDbAsync - Completed");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetUserFromDbAsync - " + ex.Message);
                throw;
            }
        }

        private async Task AddRolePermissionsForUser(UserDto user)
        {
            foreach (var role in user.Roles)
            {
                var roleWithPermissions = await GetPermissionsForRoleAsync(role.Id);
                if (roleWithPermissions?.RolePermissions != null)
                    user.UserPermissions.AddRange(roleWithPermissions.RolePermissions);
            }
        }

        private static void FilterDuplicatePermission(UserDto user)
        {
            user.UserPermissions = user.UserPermissions
                .GroupBy(p => p.PermissionId)
                .Select(g => g.First())
                .ToList();
        }

        private static void AddUiPermissionsForUser(UserDto user, List<IdentityUserUiPermission> uiPermissionsList)
        {
            foreach (var p in uiPermissionsList)
            {
                user.UserUiPermissions.Add(new UserUiPermissionDto
                {
                    UserId = p.UserId,
                    UiPermission = new UiPermission
                    {
                        PermissionId = p.PermissionId,
                        PermissionValue = p.PermissionValue,
                        PermissionDisplayName = p.PermissionDisplayName,
                        ModuleId = p.ModuleId,
                        ModuleName = p.ModuleName,
                        PermissionTypeId = p.PermissionTypeId,
                        PermissionTypeName = p.PermissionTypeName,
                        PermissionParentId = p.PermissionParentId,
                        PermissionParentName = p.PermissionParentName
                    }
                });
            }
        }

        private static void FilterDuplicatedUiPermission(UserDto user)
        {
            user.UserUiPermissions = user.UserUiPermissions
                .GroupBy(p => p.UiPermission.PermissionId)
                .Select(g => g.First())
                .ToList();
        }


        /// <summary>
        /// Add Permissions denied For User
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionIds"></param>
        /// <returns>bool</returns>
        public async Task<bool> AddPermissionsDeniedForUser(string userId, List<string> permissionIds, string createdBy)
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.AddPermissionsDeniedForUser - In process");

                //Step 4 : Update PermissionAccepted with the Permissions
                //4.i) Get permissionIds in a string
                string permissionIdsString = String.Join(",", permissionIds);

                var parameters = new
                {
                    UserId = userId,
                    PermissionIds = permissionIdsString,
                    IsAuthorized = (bool?)null,
                    OwnerId = (string?)null,
                    SysData = (string?)null,
                    TenantId = (string?)null,
                    SubTenantId = (string?)null,
                    CreatedBy = createdBy
                };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {

                    //Step 4 : Execute Scalar
                    var insertedId = await _dapperDbConnection.ExecuteScalarAsync<string>(_sqlQueries["User.PermissionDenied.Save"], parameters);

                }
                retval = true;

                //Step 6: Logging Information
                _logger.LogInformation("UserDataAccess.AddPermissionsDeniedForUser - Completed");

            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddPermissionsDeniedForUser - " + ex.Message);
                throw;
            }

            //Step 7: Return retval 
            return retval;
        }

        /// <summary>
        /// Add Permissions granted For a User
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionIds"></param>
        /// <returns>bool</returns>
        public async Task<bool> AddPermissionsGrantedForUser(string userId, List<string> permissionIds, string createdBy)
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.AddPermissionGrantedForUser - In process");

                //Step 4 : Update PermissionAccepted with the Permissions
                //4.i) Get permissionIds in a string
                string permissionIdsString = String.Join(",", permissionIds);

                var parameters = new
                {
                    UserId = userId,
                    PermissionIds = permissionIdsString,
                    IsAuthorized = (bool?)null,
                    OwnerId = (string?)null,
                    SysData = (string?)null,
                    TenantId = (string?)null,
                    SubTenantId = (string?)null,
                    CreatedBy = createdBy
                };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {

                    //Step 4 : Execute Scalar
                    var insertedId = await _dapperDbConnection.ExecuteScalarAsync<string>(_sqlQueries["User.PermissionGranted.Save"], parameters);
                    retval = true;
                }

                //Step 5: Update user object Cache


                ////Step 6: Dispatch Events
                //await DispatchEvents(new NetAuth.Domain.Entities.User());

                //Step 7: Logging Information
                _logger.LogInformation("UserDataAccess.AddPermissionGrantedForUser - Completed");

            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddPermissionGrantedForUser - " + ex.Message);
                throw;
            }

            //Step 7: Return retval 
            return retval;
        }

        /// <summary>
        /// Add permissions for role
        /// </summary>
        /// <param name="rolePermission"></param>
        /// <returns>bool</returns>
        public async Task<bool> AddPermissionsForRole(string roleId, List<string> permissionIds, string createdBy)
        {
            bool retVal = false;
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.AddPermissionForRole - In process");

                //Step 4 : Update RolePermission with the Permissions
                //4.i) Get permissionIds in a string
                string permissionIdsString = String.Join(",", permissionIds);

                //4.ii)  :Assigning values to Sql Parameters
                var parameters = new
                {
                    PermissionIds = permissionIdsString,
                    RoleId = roleId,

                    // Auditable Fields
                    IsAuthorized = (bool?)null,
                    OwnerId = (string?)null,
                    SysData = (string?)null,
                    TenantId = (string?)null,
                    SubTenantId = (string?)null,
                    CreatedBy = createdBy
                };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {

                    var insertedId = await _dapperDbConnection.ExecuteScalarAsync<string>(_sqlQueries["RolePermission.Save"], parameters);

                    retVal = true;
                }

                //Step 5: Logging Information
                _logger.LogInformation("UserDataAccess.AddPermissionForRole - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddPermissionForRole - " + ex.Message);
                throw;
            }

            //Step 6: Return retVal 
            return retVal;
        }

        /// <summary>
        /// Add Roles For User
        /// </summary>
        /// <param name="user"></param>
        /// <returns>bool</returns>
        public async Task<bool> AddRolesForUser(string userId, List<string> roleIds, string createdBy)
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.AddRolesForUser - In process");

                //Step 2: Get roleIds in a string
                string roleIdsString = String.Join(",", roleIds);

                //Step 3 :Assigning values to Sql Parameters
                var parameters = new
                {
                    RoleIds = roleIdsString,
                    UserId = userId,

                    // Auditable Fields
                    IsAuthorized = (bool?)null,
                    OwnerId = (string?)null,
                    SysData = (string?)null,
                    TenantId = (string?)null,
                    SubTenantId = (string?)null,
                    CreatedBy = createdBy
                };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {

                    //Step 5: Execute Scalar
                    var insertedId = await _dapperDbConnection.ExecuteScalarAsync<string>(_sqlQueries["User.AddRoles"], parameters);
                    retval = true;
                }

                //Step 6: Logging Information
                _logger.LogInformation("UserDataAccess.AddRolesForUser - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddRolesForUser - " + ex.Message);
                throw;
            }

            //Step 7: Return retval 
            return retval;
        }

        /// <summary>
        /// Add Role For User
        /// </summary>
        /// <param name="user"></param>
        /// <returns>bool</returns>
        public async Task<bool> AddRoleForUser(string userId, string roleId, string createdBy)
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.AddRolesForUser - In process");

                //Step 2 :Assigning values to Sql Parameters
                var parameters = new
                {
                    RoleId = roleId,
                    UserId = userId,

                    // Auditable Fields
                    IsAuthorized = (bool?)null,
                    OwnerId = (string?)null,
                    SysData = (string?)null,
                    TenantId = (string?)null,
                    SubTenantId = (string?)null,
                    CreatedBy = createdBy
                };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {

                    //Step 5: Execute Scalar
                    var insertedId = await _dapperDbConnection.ExecuteScalarAsync<string>(_sqlQueries["User.AddRole"], parameters);
                    retval = true;
                }


                //Step 4: Logging Information
                _logger.LogInformation("UserDataAccess.AddRoleForUser - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddRoleForUser - " + ex.Message);
                throw;
            }

            //Step 5: Return retval 
            return retval;
        }

        /// <summary>
        /// Delete Role For User
        /// </summary>
        /// <param name="user"></param>
        /// <returns>bool</returns>
        public async Task<bool> DeleteRoleForUser(string userId, string roleId, string createdBy)
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.DeleteRoleForUser - In process");

                //Step 2 :Assigning values to Sql Parameters
                var parameters = new
                {
                    RoleId = roleId,
                    UserId = userId,

                    // Auditable Fields
                    IsAuthorized = (bool?)null,
                    OwnerId = (string?)null,
                    SysData = (string?)null,
                    TenantId = (string?)null,
                    SubTenantId = (string?)null,
                    CreatedBy = createdBy
                };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {

                    var insertedId = await _dapperDbConnection.ExecuteScalarAsync<string>(_sqlQueries["User.DeleteRole"], parameters);
                    retval = true;

                }
                //Step 5: Logging Information
                _logger.LogInformation("UserDataAccess.DeleteRoleForUser - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.DeleteRoleForUser - " + ex.Message);
                throw;
            }

            //Step 5: Return retval 
            return retval;
        }

        /// <summary>
        /// Updates accell level of user in user table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserAccessLevel(User user)
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information In process
                _logger.LogInformation("UserDataAccess.UpdateUserAccessLevel - In process");

                //Step 2 :Assigning values to Sql Parameters
                var parameters = new
                {
                    UserId = user.Id,
                    AccessLevel = user.AccessLevel,
                    UpdatedBy = user.UpdatedBy
                };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {

                    var insertedId = await _dapperDbConnection.ExecuteScalarAsync<string>(_sqlQueries["User.UpdateAccessLevel"], parameters);
                    retval = true;

                }

                //Step 5: Logging Information
                _logger.LogInformation("UserDataAccess.UpdateUserAccessLevel - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.UpdateUserAccessLevel - " + ex.Message);
                throw;
            }

            //Step 5: Return retval 
            return retval;
        }

        /// <summary>
        ///  Get UserAccessLevel List
        /// </summary>
        /// <returns>Access Levels</returns>
        public async Task<List<NetAuth.Domain.Entities.UserAccessLevel>> GetUserAccessLevelList()
        {
            try
            {
                IEnumerable<UserAccessLevel> userAccessLevels = Enumerable.Empty<UserAccessLevel>();

                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.GetUserAccessLevelList - In process");

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    //Step 2: Execute Reader
                    userAccessLevels = await _dapperDbConnection.QueryAsync<UserAccessLevel>(_sqlQueries["UserAccessLevel.Select"]);
                }

                _logger.LogInformation("UserDataAccess.GetUserAccessLevelList - Completed");
                return userAccessLevels.ToList();
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
        public async Task<List<NetAuth.Domain.Entities.UserProfile>> GetUserProfileByUserId(string userId)
        {
            try
            {
                IEnumerable<UserProfile> userProfiles = Enumerable.Empty<UserProfile>();

                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.GetUserProfileByUserId - In process");
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@UserId", userId));

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    //Step 2: Execute Reader
                    userProfiles = await _dapperDbConnection.QueryAsync<UserProfile>(_sqlQueries["UserProfile.SelectByUserId"], new { UserId = userId });
                }

                return userProfiles.ToList();
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
        public async Task<List<NetAuth.Domain.Entities.UserProfile>> GetUserProfileByProfileId(string profileId)
        {
            try
            {
                IEnumerable<UserProfile> userProfiles = Enumerable.Empty<UserProfile>();
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.GetUserProfileByProfileId - In process");

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    //Step 2: Execute Reader
                    userProfiles = await _dapperDbConnection.QueryAsync<UserProfile>(_sqlQueries["UserProfile.SelectByProfileId"], new { ProfileId = profileId });
                }

                //Step 3: Logging Information
                _logger.LogInformation("UserDataAccess.GetUserProfileByProfileId - Completed");

                //Step 4: Return UserProfile List 
                return userProfiles.ToList();
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
        public async Task<List<NetAuth.Domain.Entities.UserProfile>> GetUserProfileList()
        {
            try
            {
                IEnumerable<UserProfile> userProfiles = Enumerable.Empty<UserProfile>();

                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.GetUserProfileList - In process");

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    //Step 2: Execute Reader
                    userProfiles = await _dapperDbConnection.QueryAsync<UserProfile>(_sqlQueries["UserProfile.Select"]);
                }

                //Step 3: Logging Information
                _logger.LogInformation("UserDataAccess.GetUserProfileList - Completed");

                //Step 4: Return UserProfile list
                return userProfiles.ToList();
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
        /// <returns>List<AuthReferenceLookup>></returns>
        public async Task<List<AuthReferenceLookup>> GetAuthReferenceLookupList(string type)
        {
            try
            {
                List<AuthReferenceLookup> authReferenceLookups = new List<AuthReferenceLookup>();
                // Step 1: Logging Information : In process
                _logger.LogInformation("UserDataAccess.AuthReferenceLookupList - In process");

                // string query = @"SELECT * FROM ""AuthReferenceLookup"" WHERE ""Type"" = @Type ORDER BY ""Name"" ASC;";
                string query = _sqlQueries["AuthReferenceLookup.SelectByType"];

                // string connectionString = _configuration["NetAuth.ConnectionStrings:PostgresSqlDBConnection"];

                //Postgre
                // string query = @"SELECT * FROM ""AuthReferenceLookup"" WHERE ""Type"" = @Type ORDER BY ""Name"" ASC;";
                //string connectionString = _configuration["NetAuth.ConnectionStrings:PostgresSqlDBConnection"]; // Make sure this key exists in config


                var parameters = new { Type = type };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    authReferenceLookups = (await _dapperDbConnection.QueryAsync<AuthReferenceLookup>(query, parameters)).AsList();
                }

                // Step 5: Logging Information: Completed
                _logger.LogInformation("UserDataAccess.AuthReferenceLookupList - Completed");

                return authReferenceLookups;
            }
            catch (Exception ex)
            {
                _logger.LogError($"UserDataAccess.AuthReferenceLookupList - {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Add permissions 
        /// </summary>
        /// <param name="permissionValue"></param>
        /// <param name="permissionDisplayName"></param>
        /// /// <param name="userName"></param>
        /// <returns>bool</returns>
        public async Task<bool> AddPermissions(string permissionValue, string permissionDisplayName, string userName)
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.AddPermissions - In process");

                var existingPermissions = await _identityManager.GetPermissionsAsync();

                var IsPermissionExists = existingPermissions.Exists(x => x.PermissionValue.Equals(permissionValue)
                                                        && x.PermissionDisplayName.Equals(permissionDisplayName));

                if (!IsPermissionExists)
                {

                    // Prepare query parameters as anonymous object
                    var parameters = new
                    {
                        PermissionValue = permissionValue,
                        PermissionDisplayName = permissionDisplayName,
                        IsAuthorized = (bool?)null,
                        OwnerId = (string?)null,
                        SysData = (string?)null,
                        TenantId = (string?)null,
                        SubTenantId = (string?)null,
                        CreatedBy = userName
                    };

                    using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                    {

                        //  Execute using Dapper 
                        var insertedId = await _dapperDbConnection.ExecuteScalarAsync<string>(
                        _sqlQueries["User.PermissionGranted.Save"],
                        parameters
                    );
                    }

                    retval = true;

                }


                ////Step 5: Dispatch Events
                //await DispatchEvents(new NetAuth.Domain.Entities.User());



                //Step 6: Logging Information
                _logger.LogInformation("UserDataAccess.AddPermissions - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddPermissions - " + ex.Message);
                throw;
            }

            //Step 7: Return retval 
            return retval;
        }


        /// <summary>
        /// Add permission
        /// </summary>
        /// <param name="permissionValue"></param>
        /// <param name="permissionDisplayName"></param>
        /// /// <param name="userName"></param>
        /// <returns>bool</returns>
        public async Task<bool> AddPermission(Permission permission, string userName)
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.AddPermissions - In process");

                var existingPermissions = await _identityManager.GetPermissionsAsync();

                var IsPermissionExists = existingPermissions.Exists(x => x.PermissionValue.Equals(permission.PermissionValue)
                                                        && x.PermissionDisplayName.Equals(permission.PermissionDisplayName));

                if (!IsPermissionExists)
                {
                    var parameters = new
                    {
                        PermissionValue = permission.PermissionValue,
                        PermissionDisplayName = permission.PermissionDisplayName,
                        PermissionSetId = permission.PermissionSetId,
                        ModuleId = permission.ModuleId,
                        PermissionType = permission.PermissionType,
                        IsActive = permission.IsActive,
                        IsAuthorized = (bool?)null,
                        OwnerId = (string?)null,
                        SysData = (string?)null,
                        TenantId = (string?)null,
                        SubTenantId = (string?)null,
                        CreatedBy = userName
                    };

                    using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                    {

                        // Step 2: Execute the mapped SQL (expected to return scalar value)
                        var insertedId = await _dapperDbConnection.ExecuteScalarAsync<string>(
                        _sqlQueries["Permission.Save"],
                        parameters
                    );
                        retval = true;
                    }
                }

                ////Step 5: Dispatch Events
                //await DispatchEvents(new NetAuth.Domain.Entities.User());


                //Step 6: Logging Information
                _logger.LogInformation("UserDataAccess.AddPermissions - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.AddPermissions - " + ex.Message);
                throw;
            }

            //Step 7: Return retval 
            return retval;
        }

        /// <summary>
        /// Update permission
        /// </summary>
        /// <param name="permissionValue"></param>
        /// <param name="permissionDisplayName"></param>
        /// /// <param name="userName"></param>
        /// <returns>bool</returns>
        public async Task<bool> UpdatePermission(Domain.Entities.UpdatePermission permission, string userName)
        {
            bool retval = false;
            try
            {
                int rowsAffected = 0;
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.UpdatePermission - In process");

                var existingPermissions = await _identityManager.GetPermissionsAsync();

                var IsPermissionExists = existingPermissions.Exists(x => x.PermissionValue.Equals(permission.PermissionValue)
                                                        && x.PermissionDisplayName.Equals(permission.PermissionDisplayName)
                                                        && !x.PermissionId.Equals(permission.Id));

                if (!IsPermissionExists)
                {
                    var parameters = new
                    {
                        Id = permission.Id,
                        PermissionValue = permission.PermissionValue,
                        PermissionDisplayName = permission.PermissionDisplayName,
                        PermissionSetId = permission.PermissionSetId,
                        PermissionType = permission.PermissionType,
                        IsActive = permission.IsActive,
                        IsDeleted = permission.IsDeleted,
                        IsApproved = permission.IsApproved,
                        ApproverId = permission.ApproverId,
                        ApprovedDateTime = permission.ApprovedDateTime,
                        IsAuthorized = permission.IsAuthorized,
                        AuthorizedById = permission.AuthorizedById,
                        AuthorizedDateTime = permission.AuthorizedDateTime,
                        ModuleId = permission.ModuleId,
                        UpdatedBy = userName,
                        UpdatedDateTime = DateTime.UtcNow
                    };

                    using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                    {
                        rowsAffected = await _dapperDbConnection.ExecuteAsync(_sqlQueries["Permission.Update"], parameters); // returns number of rows affected[4][6]
                    }

                    retval = rowsAffected > 0;
                }

                ////Step 5: Dispatch Events
                //await DispatchEvents(new NetAuth.Domain.Entities.User());


                //Step 6: Logging Information
                _logger.LogInformation("UserDataAccess.UpdatePermission - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.UpdatePermission - " + ex.Message);
                throw;
            }

            //Step 7: Return retval 
            return retval;
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
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.UserUiPermissionsByUserId - In process");

                List<UserUiPermissionDto> userUiPermissions = new List<UserUiPermissionDto>();

                // Get Ui Permission by UserId 
                var user = await GetUserFromDbAsync(userId);
                userUiPermissions = user.UserUiPermissions;

                _logger.LogInformation("UserDataAccess.UserUiPermissionsByUserId - Completed");
                return userUiPermissions;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// ActivateOrInActivateUser
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="IsActive"></param>
        /// <returns></returns>
        public async Task<int> ActivateOrInActivateUser(string userId, bool isActive)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.ActivateOrInActivateUser - In process");

                /* string query = @"
            UPDATE ""User""
            SET ""IsActive"" = @IsActive
            WHERE ""Id"" = @UserId;
        "; */
                string query = _sqlQueries["User.SetActiveStatus"];

                //Postgre
                //        string query = @"
                //    UPDATE ""User""
                //    SET ""IsActive"" = @IsActive
                //    WHERE ""Id"" = @UserId;
                //";
                var parameters = new
                {
                    UserId = userId,
                    IsActive = isActive
                };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {

                    await _dapperDbConnection.ExecuteAsync(query, parameters);

                }
                _logger.LogInformation("UserDataAccess.ActivateOrInActivateUser - Completed");

                return (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.ActivateOrInActivateUser - " + ex.Message);
                throw;
            }
        }

        #region UserActivity 

        /// <summary>
        /// Add UserActivity
        /// </summary>
        /// <param name="userActivity"></param>
        /// <returns></returns>
        public async Task<string> AddUserActivity(UserActivity userActivity)
        {
            try
            {
                IEnumerable<string> userActivityId = Enumerable.Empty<string>();

                // Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.AddUserActivity - In process");

                string insertQuery = _sqlQueries["UserActivity.Add"];
                // Step 2: Prepare parameters
                var parameters = new
                {
                    userActivity.UserId,
                    userActivity.LastLoginDateTime,
                    userActivity.LastLogoutDateTime,
                    userActivity.LastActivityDateTime,
                    userActivity.LastActivityModule,
                    LastActionType = userActivity.LastActionType?.ToString(),
                    userActivity.LastActivityDetail,
                    userActivity.CreatedBy,
                    userActivity.CreatedDateTime
                };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    userActivityId = await _dapperDbConnection.QueryAsync<string>(insertQuery, parameters).ConfigureAwait(false);
                }

                // Step 4: Logging Information
                _logger.LogInformation("UserDataAccess.AddUserActivity - Completed");

                return userActivityId.ToString();

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
        /// <returns>List<NetAuth.Domain.Entities.UserActivities></returns>
        public async Task<List<UserActivityDto>> GetUserActivities(string userId, int pageSize, int pageNumber, string startDate, string endDate)
        {
            try
            {
                IEnumerable<UserActivityDto> result = Enumerable.Empty<UserActivityDto>();
                _logger.LogInformation("UserDataAccess.GetUserActivities - In process");

                pageNumber = pageNumber <= 0 ? 1 : pageNumber;
                pageSize = pageSize <= 0 ? 10 : pageSize;

                string query = _sqlQueries["UserActivity.GetUserActivities"];

                var parameters = new
                {
                    UserId = string.IsNullOrEmpty(userId) || userId == "0" ? null : userId,
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    StartDate = startDate ?? "",
                    EndDate = endDate ?? ""
                };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {

                    result = await _dapperDbConnection.QueryAsync<UserActivityDto>(query, parameters);
                }

                _logger.LogInformation("UserDataAccess.GetUserActivities - Completed");

                return result.ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError($"UserDataAccess.GetUserActivities - {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get UserActivities By UserIds
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns>List<NetAuth.Domain.Entities.UserActivities></returns>
        public async Task<List<UserActivity>> GetUserActivitiesByUserIds(string userIds)
        {
            try
            {
                IEnumerable<UserActivity> result = Enumerable.Empty<UserActivity>();

                _logger.LogInformation("UserDataAccess.GetUserActivitiesByUserIds - In process");

                /* string query = @"
            WITH ranked_activity AS (
                SELECT
                    ROW_NUMBER() OVER (PARTITION BY ""UserId"" ORDER BY ""LastActivityDateTime"" DESC) AS rank,
                    ""Id"", ""UserId"", ""LastLoginDateTime"", ""LastLogoutDateTime"",
                    ""LastActivityDateTime"", ""LastActivityModule"", ""LastActionType"",
                    ""LastActivityDetail"", ""CreatedBy"", ""CreatedDateTime"",
                    ""UpdatedBy"", ""UpdatedDateTime"", ""UpdateReason"", ""OwnerId"",
                    ""IsActive"", ""IsDeleted"", ""IsApproved"", ""ApproverId"",
                    ""ApprovedDateTime"", ""IsAuthorized"", ""AuthorizedById"",
                    ""AuthorizedDateTime"", ""TenantId"", ""SubTenantId"",
                    ""SysData"", ""CustomFields""
                FROM ""UserActivity""
                WHERE ""UserId"" = ANY(string_to_array(@UserIds, ','))
            )
            SELECT * FROM ranked_activity WHERE rank = 1;
        "; */
                string query = _sqlQueries["UserActivity.GetLatestByUserIds"];

                var parameters = new { UserIds = userIds };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {

                    result = await _dapperDbConnection.QueryAsync<UserActivity>(query, parameters);

                }
                _logger.LogInformation("UserDataAccess.GetUserActivitiesByUserIds - Completed");

                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"UserDataAccess.GetUserActivitiesByUserIds - {ex.Message}");
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Update UserPasswordHash
        /// </summary>
        /// <param name="userPasswordHash"></param>
        /// <returns>int</returns>
        public async Task<int> UpdateUserPasswordHash(UserPasswordHash userPasswordHash)
        {
            try
            {
                int rowsAffected = 0;

                _logger.LogInformation("UserDataAccess.UpdateUserPasswordHash - In process");

                if (string.IsNullOrWhiteSpace(userPasswordHash.UserId))
                {
                    throw new ApplicationException("UserId cannot be null or empty!");
                }

                if (string.IsNullOrWhiteSpace(userPasswordHash.PasswordHash))
                {
                    throw new ApplicationException("PasswordHash cannot be null or empty!");
                }

                /* var query = @"
                    UPDATE ""UserPasswordHash""
                        SET
                        ""PasswordHash""     = @PasswordHash,
                        ""UpdatedBy""        = @UpdatedBy,
                        ""UpdatedDateTime""  = @UpdatedDateTime,
                        ""UpdateReason""     = @UpdateReason
                    WHERE ""UserId"" = @UserId"; */
                var query = _sqlQueries["UserPasswordHash.Update"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {

                    rowsAffected = await _dapperDbConnection.ExecuteAsync(query, new
                    {
                        userPasswordHash.UserId,
                        userPasswordHash.PasswordHash,
                        userPasswordHash.UpdatedBy,
                        userPasswordHash.UpdatedDateTime,
                        userPasswordHash.UpdateReason
                    });
                }
                _logger.LogInformation("UserDataAccess.UpdateUserPasswordHash - Completed");

                return rowsAffected;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.UpdateUserPasswordHash - " + ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Get UserPasswordHash
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        public async Task<UserPasswordHash> GetUserPasswordHash(string userId)
        {
            try
            {
                UserPasswordHash result = new UserPasswordHash();
                _logger.LogInformation("UserDataAccess.GetUserPasswordHash - In process");

                // var conString = _configuration["NetAuth.ConnectionStrings:PostgresDBConnection"];

                /* var query = @"
                            SELECT
                                ""UserId"",
                                ""PasswordHash"",
                                ""UpdatedBy"",
                                ""UpdatedDateTime"",
                                ""UpdateReason""
                            FROM ""UserPasswordHash""
                            WHERE ""UserId"" = @UserId"; */
                var query = _sqlQueries["UserPasswordHash.SelectByUserId"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    result = await _dapperDbConnection.QueryFirstOrDefaultAsync<UserPasswordHash>(
                   query,
                   new { UserId = userId }
               );

                }
                _logger.LogInformation("UserDataAccess.GetUserPasswordHash - Completed");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.GetUserPasswordHash - " + ex.Message);
                throw;
            }
        }

        public async Task<int> UpdateUser(Domain.Entities.User updateUser)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.UpdateUser - In process");

                /* string query = @"
            UPDATE ""User""
            SET
                ""Email"" = @Email,
                ""PhoneNumber"" = @PhoneNumber,
                ""EmpId"" = @EmpId
            WHERE ""Id"" = @UserId;
        "; */
                string query = _sqlQueries["User.UpdateBasicInfo"];

                var parameters = new
                {
                    UserId = updateUser.Id,
                    Email = updateUser.Email,
                    PhoneNumber = updateUser.PhoneNumber,
                    EmpId = updateUser.EmpId
                };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    await _dapperDbConnection.ExecuteAsync(query, parameters);
                }
                _logger.LogInformation("UserDataAccess.UpdateUser - Completed");

                return (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.UpdateUser - " + ex.Message);
                throw;
            }
        }

        #region Team management

        public async Task<List<NetAuth.Domain.Dto.TeamDto>> GetTeams()
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetTeams - In process");
                var teams = new List<NetAuth.Domain.Dto.TeamDto>();

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    teams = (await _dapperDbConnection.QueryAsync<NetAuth.Domain.Dto.TeamDto>(_sqlQueries["Team.Select"])).ToList();
                }

                _logger.LogInformation("UserDataAccess.GetTeams - Completed");
                return teams;
            }
            catch (Exception ex) { _logger.LogError($"UserDataAccess.GetTeams - {ex.Message}"); throw; }
        }

        public async Task<NetAuth.Domain.Dto.TeamDto> GetTeamById(string teamId)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetTeamById - In process");
                NetAuth.Domain.Dto.TeamDto team = null;

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    using var multi = await _dapperDbConnection.QueryMultipleAsync(_sqlQueries["Team.SelectById"], new { TeamId = teamId });
                    team = (await multi.ReadAsync<NetAuth.Domain.Dto.TeamDto>()).FirstOrDefault();
                    var memberIds = (await multi.ReadAsync<string>()).ToList();
                    if (team != null)
                        team.MemberIds = memberIds;
                }

                if (team == null) return null;

                _logger.LogInformation("UserDataAccess.GetTeamById - Completed");
                return team;
            }
            catch (Exception ex) { _logger.LogError($"UserDataAccess.GetTeamById - {ex.Message}"); throw; }
        }

        public async Task<string> AddTeam(NetAuth.Domain.Dto.TeamDto team, string createdBy)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.AddTeam - In process");
                string newId;
                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    newId = await _dapperDbConnection.QuerySingleAsync<string>(_sqlQueries["Team.Save"],
                        new { team.TeamName, team.TeamShortName, team.Description, team.TeamOwnerId, team.TeamCaptainId, CreatedBy = createdBy });
                }
                var initialMembers = new List<string> { team.TeamOwnerId, team.TeamCaptainId }
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Distinct()
                    .ToList();
                if (initialMembers.Count > 0)
                    await AddTeamMembers(newId, initialMembers, createdBy);
                _logger.LogInformation("UserDataAccess.AddTeam - Completed");
                return newId;
            }
            catch (Exception ex) { _logger.LogError($"UserDataAccess.AddTeam - {ex.Message}"); throw; }
        }

        public async Task<bool> AddTeamMembers(string teamId, List<string> userIds, string createdBy)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.AddTeamMembers - In process");
                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    await _dapperDbConnection.ExecuteAsync(_sqlQueries["TeamMember.Save"],
                        new { TeamId = teamId, MemberIds = string.Join(",", userIds), CreatedBy = createdBy });
                }
                _logger.LogInformation("UserDataAccess.AddTeamMembers - Completed");
                return true;
            }
            catch (Exception ex) { _logger.LogError($"UserDataAccess.AddTeamMembers - {ex.Message}"); throw; }
        }

        public async Task<bool> RemoveTeamMember(string teamId, string userId)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.RemoveTeamMember - In process");
                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    await _dapperDbConnection.ExecuteAsync(_sqlQueries["TeamMember.Delete"],
                        new { TeamId = teamId, MemberId = userId });
                }
                _logger.LogInformation("UserDataAccess.RemoveTeamMember - Completed");
                return true;
            }
            catch (Exception ex) { _logger.LogError($"UserDataAccess.RemoveTeamMember - {ex.Message}"); throw; }
        }

        public async Task<List<NetAuth.Domain.Dto.TeamDto>> GetTeamsByUserId(string userId)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetTeamsByUserId - In process");
                var teams = new List<NetAuth.Domain.Dto.TeamDto>();

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    teams = (await _dapperDbConnection.QueryAsync<NetAuth.Domain.Dto.TeamDto>(_sqlQueries["Team.SelectByUserId"], new { UserId = userId })).ToList();
                }

                _logger.LogInformation("UserDataAccess.GetTeamsByUserId - Completed");
                return teams;
            }
            catch (Exception ex) { _logger.LogError($"UserDataAccess.GetTeamsByUserId - {ex.Message}"); throw; }
        }

        

        public async Task<List<NetAuth.Domain.Dto.TeamMemberDto>> GetTeamMembersByTeamId(string teamId)
        {
            try
            {
                _logger.LogInformation("UserDataAccess.GetTeamMembersByTeamId - In process");
                List<NetAuth.Domain.Dto.TeamMemberDto> members;

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    members = (await _dapperDbConnection.QueryAsync<NetAuth.Domain.Dto.TeamMemberDto>(
                        _sqlQueries["TeamMember.SelectByTeamId"], new { TeamId = teamId })).ToList();
                }

                _logger.LogInformation("UserDataAccess.GetTeamMembersByTeamId - Completed");
                return members;
            }
            catch (Exception ex) { _logger.LogError($"UserDataAccess.GetTeamMembersByTeamId - {ex.Message}"); throw; }
        }

        #endregion

        public async Task<List<UsersDto>> GetUsersByStatus(string status)
        {
            try
            {
                // Step 1: Logging Information: In process
                _logger.LogInformation("UserDataAccess.GetUsersByStatus - In process");

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    var query = _sqlQueries["User.GetUsersAndRoles"];
                    List<UsersDto> users = new List<UsersDto>();

                    using (var multi = await _dapperDbConnection.QueryMultipleAsync(query))
                    {
                        var userList = (await multi.ReadAsync<UsersDto>()).ToList();
                        var roleList = (await multi.ReadAsync<UserRoleDto>()).ToList();

                        // Initialize roles
                        userList.ForEach(u => u.Roles = new List<string>());

                        // Map roles to users
                        foreach (var role in roleList)
                        {
                            var user = userList.FirstOrDefault(u => u.userId == role.UserId);

                            if (user != null)
                            {
                                user.Roles.Add(role.RoleName);
                            }
                        }

                        users = userList;
                    }

                    // User Domain filters
                    if (users == null || !users.Any())
                    {
                        _logger.LogWarning("No users found.");
                        return new List<UsersDto>();
                    }

                    List<UsersDto> filteredUsers = new List<UsersDto>();

                    if (!string.IsNullOrWhiteSpace(status) &&
                        status.Equals("all", StringComparison.OrdinalIgnoreCase))
                    {
                        filteredUsers = users;
                    }
                    else if (!string.IsNullOrWhiteSpace(status) &&
                             status.Equals("active", StringComparison.OrdinalIgnoreCase))
                    {
                        filteredUsers = users.Where(x => x.IsActive).ToList();
                    }
                    else if (!string.IsNullOrWhiteSpace(status) &&
                             status.Equals("inactive", StringComparison.OrdinalIgnoreCase))
                    {
                        filteredUsers = users.Where(x => !x.IsActive).ToList();
                    }
                    else
                    {
                        _logger.LogWarning($"Invalid status '{status}' provided in the request.");
                        filteredUsers = new List<UsersDto>();
                    }

                    // Step 2: Logging Information: Completed
                    _logger.LogInformation("UserDataAccess.GetUsersByStatus - Completed");

                    return filteredUsers;
                }

            }
            catch (Exception ex)
            {
                //Step 1: Logging Error
                _logger.LogError("UserDataAccess.GetUsersByStatus - " + ex.Message);
                throw;
            }

        }
    }
}




