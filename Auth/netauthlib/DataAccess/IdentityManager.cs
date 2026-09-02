using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetAuth.Application.Common.Interfaces;
using NetAuth.Domain.Dto;
using NetAuth.Domain.Entities;
using NetAuth.Interfaces;
using NetAuth.Lib.Domain.Entities.Entities;

namespace NetAuth.DataAccess
{
    /// <summary>
    /// Data Access Layer
    /// </summary>
    internal class IdentityManager : IIdentityManager
    {
        private readonly ILogger<IdentityManager> _logger;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _sqlQueries;
        private readonly IConnectionHelper _connectionHelper;


        /// <summary>
        /// Instantiation of IdentityManager class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public IdentityManager(IConfiguration configuration, ILogger<IdentityManager> logger, IConnectionHelper connectionHelper)
        {
            this._logger = logger;
            this._configuration = configuration;

            this._connectionHelper = connectionHelper;
            this._sqlQueries = _connectionHelper.LoadSqlQueriesXml("Auth");
        }

        /// <summary>
        /// GetIdentityUserAsync
        /// </summary>
        /// <param name="userName_userId_userOid"></param>
        /// <returns>NetAuth.Domain.Entities.IdentityUser</returns>
        public async Task<IdentityUser> GetIdentityUserAsync(string userName_userId_userOid)
        {
            IdentityUser user;
            try
            {
                //Step 1 : Logging  in process
                _logger.LogInformation("IdentityManager.GetIdentityUserAsync - In process");

                //Step 2 : Get IdentityUser From db
                user = await GetIdentityUserFromDbAsync(userName_userId_userOid);

                //Step 3 : Logging Completed
                _logger.LogInformation("IdentityManager.GetIdentityUserAsync - Completed");

                //Step 4 : return user
                return user;
            }
            catch (Exception ex)
            {
                //Step 5 : throw exception
                _logger.LogError("IdentityManager.GetIdentityUserAsync - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// GetUsersAsync
        /// </summary>
        /// <returns>List<NetAuth.Domain.Entities.User></returns>
        public async Task<List<UserDto>> GetUsersAsync()
        {
            try
            {
                //Step 1 : Logging  in process
                _logger.LogInformation("IdentityManager.GetUsersAsync - In process");

                //Step 2 : Get Users from db
                List<UserDto> users = await AuthGetUsersFromDbAsync();

                //Step 3 : Logging completed
                _logger.LogInformation("IdentityManager.GetUsersAsync - Completed");

                //Step 4 : return Users
                return users;
            }
            catch (Exception ex)
            {
                //Step 5 : throw exception
                _logger.LogError("IdentityManager.GetUsersAsync - " + ex.Message);
                throw;
            }
        }
        public async Task<RoleDto> GetPermissionsForRoleAsync(string roleId)
        {
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.PermissionsForRoleAsync - In process");


                string query = _sqlQueries["RolePermissionInfo.Select"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {

                    // Execute query with Dapper's QueryMultiple to get multiple result sets
                    var rolePermission = await _dapperDbConnection.QueryAsync<NetAuth.Domain.Entities.Permission>(
                    query,
                    new { RoleId = roleId }
                    );


                    var role = new RoleDto();
                    role.Id = roleId;

                    role.RolePermissions = rolePermission.ToList();

                    //Step 5 : Logging Completed
                    _logger.LogInformation("IdentityManager.GetPermissionsForRoleAsync - Completed");

                    //Step 6 : Return role & its permissions
                    return role;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.PermissionsForRoleAsync - " + ex.Message);
                throw;
            }
        }

        public async Task<List<UserDto>> AuthGetUsersFromDbAsync()
        {
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("IdentityManager.AuthGetUsersFromDbAsync - In process");


                List<UserDto> users = new List<UserDto>();

                List<IdentityUserRole> roleList;
                List<IdentityUserTeam> userTeamList;
                List<UserPermission> userPermissionsGrantedList;
                List<UserPermission> userPermissionsDeniedList;
                List<IdentityUserUiPermission> uiUserPermissionsList;

                // Use a local connection so GetPermissionsForRoleAsync (which uses _dapperDbConnection)
                // does not conflict with the open GridReader.
                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    using var multi = await _dapperDbConnection.QueryMultipleAsync(_sqlQueries["UserInfo.Select"], new { userId = "0" });

                    users.AddRange((await multi.ReadAsync<UserDto>()).ToList());
                    roleList = (await multi.ReadAsync<IdentityUserRole>()).ToList();
                    userPermissionsGrantedList = (await multi.ReadAsync<UserPermission>()).ToList();
                    userPermissionsDeniedList = (await multi.ReadAsync<UserPermission>()).ToList();
                    uiUserPermissionsList = (await multi.ReadAsync<IdentityUserUiPermission>()).ToList();
                    // Result set 6: User Teams; Result set 7: Team Permissions — must match SQL order
                    userTeamList = (await multi.ReadAsync<IdentityUserTeam>()).ToList();
                }

                //Populate the role
                AddRoleForUser(users, roleList);

                // Add role permissions, then filter UserDenied and deduplicate
                await AddRolePermissionForUser(users);

                // User Teams
                AddTeamForUser(users, userTeamList);


                //userPermissionsGrantedList
                AddIndividuallyGrantedPermissionForUser(users, userPermissionsGrantedList);

                //userPermissionsDeniedList
                AddIndividuallyDeniedPermissionForUser(users, userPermissionsDeniedList);

                //FilterDuplicateAndDeniedPermission
                FilterDuplicateAndDeniedPermission(users);

                // UserUiPermissions (Role + UserGranted + UserDenied + Team — merged and deduplicated by SQL)
                AddUiPermissionForUser(users, uiUserPermissionsList);

                // Remove duplicate entries from UserUiPermissions
                FilterDuplicateUiPermission(users);

                //Step 4: Logging Information Completed
                _logger.LogInformation("IdentityManager.AuthGetUsersFromDbAsync - Completed");

                //Step 5: Return user
                return users;
            }
            catch (Exception ex)
            {
                //Step 6: throw exception
                _logger.LogError("IdentityManager.AuthGetUsersFromDbAsync - " + ex.Message);
                throw;
            }
        }

        public async Task<IdentityUser> GetIdentityUserFromDbAsync(string userName_userId_userOid)
        {
            try
            {
                IdentityUser identityUser = new IdentityUser();
                List<Permission> permissionsGranted = new List<Permission>();
                List<Permission> userPermissionsDenied = new List<Permission>();

                _logger.LogInformation("IdentityManager.GetIdentityUserFromDbAsync - In process");

                string query = _sqlQueries["Identity.Identity_SelectUser"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Execute query with Dapper's QueryMultiple to get multiple result sets
                    using var multi = await _dapperDbConnection.QueryMultipleAsync(
                                  query,
                                  new { userId = userName_userId_userOid }
                    );

                    // Read the first result set (user information)
                    identityUser = (await multi.ReadAsync<IdentityUser>()).FirstOrDefault();

                    if (identityUser == null)
                    {
                        _logger.LogInformation($"IdentityManager.GetIdentityUserFromDbAsync - No user found for {userName_userId_userOid}"); // No user found
                        return null;
                    }

                    // Read subsequent result sets into respective lists
                    identityUser.UserRoles = (await multi.ReadAsync<IdentityUserRole>()).ToList();
                    permissionsGranted = (await multi.ReadAsync<Permission>()).ToList();
                    userPermissionsDenied = (await multi.ReadAsync<Permission>()).ToList();


                    // If uiUserPermissionsList is also needed: var uiUserPermissionsList = (await multi.ReadAsync<UiPermission>()).ToList();
                }

                identityUser.UserPermissions = identityUser.UserPermissions ?? new List<Permission>(); // Ensure permissions list is initialized

                identityUser.UserPermissions.AddRange(permissionsGranted); // Add granted permissions


                // Add role permissions to UserPermissions
                foreach (IdentityUserRole userRole in identityUser.UserRoles)
                {
                    RoleDto roleWithPermissions = await GetPermissionsForRoleAsync(userRole.Id); // Get permissions for each role
                    if (roleWithPermissions != null && roleWithPermissions.RolePermissions != null)
                    {
                        identityUser.UserPermissions.AddRange(roleWithPermissions.RolePermissions);
                    }
                }

                // Remove denied permissions and duplicates from UserPermissions
                identityUser.UserPermissions = identityUser.UserPermissions
                    .Where(up => !userPermissionsDenied.Any(pd => pd.PermissionId == up.PermissionId)) // Filter out denied permissions
                    .GroupBy(p => p.PermissionId) // Group to remove duplicates
                    .Select(g => g.First()) // Select first of each group
                    .ToList();

                _logger.LogInformation("IdentityManager.GetIdentityUserFromDbAsync - Completed"); // Logging completion

                return identityUser; // Return IdentityUser

            }
            catch (Exception ex)
            {
                _logger.LogError($"IdentityManager.GetIdentityUserFromDbAsync - {ex.Message}"); // Log exception
                throw; // Rethrow exception
            }
        }


        #region user load sub methods
        //AddRoleForUser
        private static void AddRoleForUser(List<UserDto> users, List<IdentityUserRole> roleList)
        {
            foreach (var role in roleList)
            {
                var user = users.Find(u => u.Id.Equals(role.UserId));

                var userRole = new RoleDto();
                userRole.RolePermissions = new List<Permission>();
                userRole.RoleName = role.RoleName;
                userRole.Id = role.Id;
                user.Roles.Add(userRole);
            }
        }

        //AddRolePermissionForUser
        private async Task AddRolePermissionForUser(List<UserDto> users)
        {
            foreach (var user in users)
            {
                foreach (RoleDto userRolesOuter in user.Roles)
                {
                    RoleDto innerRole = await GetPermissionsForRoleAsync(userRolesOuter.Id);
                    if (innerRole?.RolePermissions != null)
                        user.UserPermissions.AddRange(innerRole.RolePermissions);
                }
            }
        }
        //AddTeamForUser
        private static void AddTeamForUser(List<UserDto> users, List<IdentityUserTeam> userTeamList)
        {
            foreach (var ut in userTeamList)
            {
                var user = users.Find(u => u.Id.Equals(ut.MemberId));
                if (user == null) continue;
                user.Teams.Add(new TeamDto { Id = ut.Id, TeamName = ut.TeamName, TeamShortName = ut.TeamShortName, Description = ut.Description, TeamOwnerId = ut.TeamOwnerId, TeamCaptainId = ut.TeamCaptainId });
            }
        }

        

        //AddIndividuallyGrantedPermissionForUser
        private static void AddIndividuallyGrantedPermissionForUser(List<UserDto> users, List<UserPermission> userPermissionsGrantedList)
        {
            foreach (var userPermission in userPermissionsGrantedList)
            {
                var user = users.Find(u => u.Id.Equals(userPermission.UserId));

                if (user == null) continue;

                var permission = new NetAuth.Domain.Entities.Permission();
                permission.PermissionId = userPermission.PermissionId;
                permission.PermissionValue = userPermission.PermissionValue;
                permission.PermissionDisplayName = userPermission.PermissionDisplayName;
                permission.PermissionSetId = userPermission.PermissionSetId;
                permission.PermissionSetName = userPermission.PermissionSetName;
                permission.PermissionType = userPermission.PermissionType;
                permission.ModuleId = userPermission.ModuleId;
                permission.ModuleName = userPermission.ModuleName;

                user.PermissionsGranted.Add(permission);
                user.UserPermissions.Add(permission);
            }
        }

        //AddIndividuallyDeniedPermissionForUser
        private static void AddIndividuallyDeniedPermissionForUser(List<UserDto> users, List<UserPermission> userPermissionsDeniedList)
        {
            //permissionsDeniedList
            foreach (var userPermission in userPermissionsDeniedList)
            {
                var user = users.Find(u => u.Id.Equals(userPermission.UserId));

                if (user == null) continue;

                var permission = new NetAuth.Domain.Entities.Permission();
                permission.PermissionId = userPermission.PermissionId;
                permission.PermissionValue = userPermission.PermissionValue;
                permission.PermissionDisplayName = userPermission.PermissionDisplayName;
                permission.PermissionSetId = userPermission.PermissionSetId;
                permission.PermissionSetName = userPermission.PermissionSetName;
                permission.PermissionType = userPermission.PermissionType;
                permission.ModuleId = userPermission.ModuleId;
                permission.ModuleName = userPermission.ModuleName;

                user.PermissionsDenied.Add(permission);
            }
        }

        //FilterDuplicateAndDeniedPermission
        private static void FilterDuplicateAndDeniedPermission(List<UserDto> users)
        {
            foreach (var user in users)
            {
                user.UserPermissions = user.UserPermissions
                    .Where(p => !user.PermissionsDenied.Any(d => d.PermissionId == p.PermissionId))
                    .GroupBy(p => p.PermissionId)
                    .Select(g => g.First())
                    .ToList();
            }
        }

        //FilterDuplicateUiPermission
        private static void FilterDuplicateUiPermission(List<UserDto> users)
        {
            foreach (var user in users)
            {
                user.UserUiPermissions = user.UserUiPermissions
                    .GroupBy(p => p.UiPermission.PermissionId)
                    .Select(g => g.First())
                    .ToList();
            }
        }

        private static void AddUiPermissionForUser(List<UserDto> users, List<IdentityUserUiPermission> uiUserPermissionsList)
        {
            foreach (var userUiPermission in uiUserPermissionsList)
            {
                var user = users.Find(u => u.Id.Equals(userUiPermission.UserId));

                if (user == null) continue;

                var uiPermission = new NetAuth.Domain.Dto.UserUiPermissionDto();
                uiPermission.UiPermission = new UiPermission();

                uiPermission.UserId = userUiPermission.UserId;
                uiPermission.UiPermission.PermissionId = userUiPermission.PermissionId;
                uiPermission.UiPermission.PermissionValue = userUiPermission.PermissionValue;
                uiPermission.UiPermission.PermissionDisplayName = userUiPermission.PermissionDisplayName;
                uiPermission.UiPermission.ModuleId = userUiPermission.ModuleId;
                uiPermission.UiPermission.ModuleName = userUiPermission.ModuleName;
                uiPermission.UiPermission.PermissionTypeId = userUiPermission.PermissionTypeId;
                uiPermission.UiPermission.PermissionTypeName = userUiPermission.PermissionTypeName;
                uiPermission.UiPermission.PermissionParentId = userUiPermission.PermissionParentId;
                uiPermission.UiPermission.PermissionParentName = userUiPermission.PermissionParentName;

                user.UserUiPermissions.Add(uiPermission);
            }
        }

        #endregion 



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
                //Step 1 : Logging  in process
                _logger.LogInformation("UserDataAccess.AuthHasRequestPermissionAsync - In process. UserId:" + userId + " permissionValue:" + permissionValue);
                IdentityUser user = null;

                //Step 2: Get User By userId
                user = await GetIdentityUserAsync(userId);

                //Step 3 : Check if the permission is associated with the User 
                bool hasRequestPermission = user.UserPermissions.Any(p => p.PermissionValue.Equals(permissionValue));

                //3.1 If Permission is not asscociated with User, then check if the permission exists in table
                if (!hasRequestPermission)
                {
                    //Step 4 : Get all permissions
                    List<Permission> permissions = await GetPermissionsAsync();

                    //Step 5 : Check if permission exists in permission table
                    if (permissions.Any(p => p.PermissionValue.Equals(permissionValue)))
                    {
                        // 5.1 Exists : then deny access to User
                        _logger.LogInformation("IdentityManager.AuthHasRequestPermissionAsync - Completed");
                        return false;
                    }
                    else
                    {
                        // 5.2 Not exists : allow access to User
                        _logger.LogInformation("IdentityManager.AuthHasRequestPermissionAsync - Completed");
                        return true;
                    }
                }
                else
                {
                    //3.2 If permission is associated with User, return true
                    _logger.LogInformation("IdentityManager.AuthHasRequestPermissionAsync - Completed");
                    return hasRequestPermission;
                }
                ;
            }
            catch (Exception ex)
            {
                //Step 6 : throw exception
                _logger.LogError("UserDataAccess.AuthHasRequestPermissionAsync - " + ex.Message + " UserId:" + userId + " permissionValue: " + permissionValue);
                throw;
            }
        }

        /// <summary>
        /// GetUserByRoleIdAsync
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<List<UserDto>> GetUserByRoleIdAsync(string roleId)
        {
            try
            {
                //Step 1 : Logging  in process
                _logger.LogInformation("IdentityManager.GetUserByRoleIdAsync - In process");

                //Step 2 : Check Users collection first and make use of caching
                List<UserDto> users = await GetUsersAsync();

                //Step 3 : Get Users by RoleId
                if (users.Any(u => u.Roles.Exists(r => r.Id.Equals(roleId))))
                {
                    List<UserDto> userByRoleId = users.FindAll(u => u.Roles.Exists(r => r.Id.Equals(roleId)));

                    _logger.LogInformation("IdentityManager.AuthHasRequestPermissionAsync - Completed");
                    return userByRoleId;
                }
                else
                {
                    _logger.LogInformation("IdentityManager.GetUserByRoleIdAsync - Completed");
                    return null;
                }
            }
            catch (Exception ex)
            {
                //Step 4 : throw exception
                _logger.LogError("UserDataAccess.GetUserByRoleId - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get Permissions
        /// </summary>
        /// <returns>Permission List</returns>
        public async Task<List<Permission>> GetPermissionsAsync()
        {
            try
            {
                //Step 1 : Logging  in process
                _logger.LogInformation("IdentityManager.GetPermissionsAsync - In process");

                //Step 2: call DB for permissions
                List<Permission> permissionsList = await GetPermissionsFromDBAsync();

                //Step 3: Logging Information Completed
                _logger.LogInformation("IdentityManager.GetPermissionsAsync - Completed");

                //Step 4: Return permissions
                return permissionsList;
            }
            catch (Exception ex)
            {
                //Step 5 : throw exception
                _logger.LogError("IdentityManager.GetPermissionsAsync - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get all Permissions, active and inactive
        /// </summary>
        /// <returns>Permission List</returns>
        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            try
            {
                //Step 1 : Logging  in process
                _logger.LogInformation("IdentityManager.GetAllPermissionsAsync - In process");

                //Step 2: call DB for permissions
                List<Permission> permissionsList = await GetAllPermissionsFromDBAsync();

                //Step 3: Logging Information Completed
                _logger.LogInformation("IdentityManager.GetAllPermissionsAsync - Completed");

                //Step 4: Return permissions
                return permissionsList;
            }
            catch (Exception ex)
            {
                //Step 5 : throw exception
                _logger.LogError("IdentityManager.GetAllPermissionsAsync - " + ex.Message);
                throw;
            }
        }

        #region DB call methods are abstracted. So that, one[Non Auth Dev] can not use/refrence them.
        private async Task<List<Permission>> GetPermissionsFromDBAsync()
        {
            try
            {
                _logger.LogInformation("IdentityManager.GetPermissionsFromDBAsync - In process");

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    var permissionList = await _dapperDbConnection.QueryAsync<NetAuth.Domain.Entities.Permission>(_sqlQueries["Permission.Select"]);

                    _logger.LogInformation("IdentityManager.GetPermissionsFromDBAsync - Completed");

                    return permissionList.ToList();
                }
            }
            catch (Exception ex)
            {
                //Step 7: throw exception
                _logger.LogError("IdentityManager.GetPermissionsAsync - " + ex.Message);
                throw;
            }
        }

        private async Task<List<Permission>> GetAllPermissionsFromDBAsync()
        {
            try
            {
                _logger.LogInformation("IdentityManager.GetAllPermissionsFromDBAsync - In process");

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    var permissionList = await _dapperDbConnection.QueryAsync<NetAuth.Domain.Entities.Permission>(_sqlQueries["Permission.SelectAll"]);

                    _logger.LogInformation("IdentityManager.GetAllPermissionsFromDBAsync - Completed");

                    return permissionList.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("IdentityManager.GetAllPermissionsAsync - " + ex.Message);
                throw;
            }
        }
        #endregion
    }
}
