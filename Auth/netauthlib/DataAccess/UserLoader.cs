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
    ///  /// Data Access layer :where we write code to connect DB and fetch or manipulate records from DB.
    /// In the database layer, we'll find things like database, connection, table, SQL, and result set.
    /// </summary>
    internal class UserLoader : IUserLoader
    {
        private ILogger<UserLoader> _logger;
        private IConfiguration _configuration;
        private readonly Dictionary<string, string> _sqlQueries;
        private readonly IConnectionHelper _connectionHelper;

        public UserLoader(IConfiguration configuration, ILogger<UserLoader> logger, IConnectionHelper connectionHelper)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._connectionHelper = connectionHelper;
            this._sqlQueries = _connectionHelper.LoadSqlQueriesXml("Auth");


        }

        #region Async Method []

        public async Task<List<UserDto>> LoadUsersFromDbAsync(string paramUserId)
        {
            try
            {
                _logger.LogInformation("IdentityManager.AuthGetUsersFromDbAsync - In process");


                if (string.IsNullOrEmpty(paramUserId)) return null;

                List<UserDto> users = new List<UserDto>();
                List<IdentityUserRole> userRoleList = new List<IdentityUserRole>();
                List<UserPermission> userGrantedPermissionList = new List<UserPermission>();
                List<UserPermission> userDeniedPermissionList = new List<UserPermission>();
                List<IdentityUserUiPermission> userUiPermissionList = new List<IdentityUserUiPermission>();


                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    var multi = await _dapperDbConnection.QueryMultipleAsync(
                    _sqlQueries["UserInfo.Select"],
                    new { userId = paramUserId });


                    var userList = (await multi.ReadAsync<UserDto>()).ToList();
                    users.AddRange(userList);

                    userRoleList = (await multi.ReadAsync<IdentityUserRole>()).ToList();
                    userGrantedPermissionList = (await multi.ReadAsync<UserPermission>()).ToList();
                    userDeniedPermissionList = (await multi.ReadAsync<UserPermission>()).ToList();
                    userUiPermissionList = (await multi.ReadAsync<Lib.Domain.Entities.Entities.IdentityUserUiPermission>()).ToList();
                }

                // Mapping
                LoadUserRoles(users, userRoleList);
                LoadUserGrantedPermissions(users, userGrantedPermissionList);
                LoadUserDeniedPermissions(users, userDeniedPermissionList);
                await LoadUserRolePermissionsAsync(users);
                LoadUserUiPermissions(users, userUiPermissionList);

                _logger.LogInformation("IdentityManager.AuthGetUsersFromDbAsync - Completed");

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError("IdentityManager.AuthGetUsersFromDbAsync - " + ex.Message);
                throw;
            }
        }

        private void LoadUserRoles(List<UserDto> users, List<IdentityUserRole> roles)
        {
            foreach (var role in roles)
            {
                var user = users.Find(u => u.Id == role.UserId);
                if (user == null) continue;

                user.Roles.Add(new RoleDto
                {
                    Id = role.Id,
                    RoleName = role.RoleName,
                    RolePermissions = new List<Permission>()
                });
            }
        }

        private void LoadUserGrantedPermissions(List<UserDto> users, List<UserPermission> permissions)
        {
            foreach (var p in permissions)
            {
                var user = users.Find(u => u.Id == p.UserId);
                if (user == null) continue;

                var permission = new Permission
                {
                    PermissionId = p.PermissionId,
                    PermissionValue = p.PermissionValue,
                    PermissionDisplayName = p.PermissionDisplayName,
                    PermissionSetId = p.PermissionSetId,
                    PermissionSetName = p.PermissionSetName,
                    ModuleId = p.ModuleId,
                    ModuleName = p.ModuleName,
                    ApiName = p.ApiName,
                    IsActive = p.IsActive
                };

                user.PermissionsGranted.Add(permission);
                user.UserPermissions.Add(permission);
            }
        }

        private void LoadUserDeniedPermissions(List<UserDto> users, List<UserPermission> permissions)
        {
            foreach (var p in permissions)
            {
                var user = users.Find(u => u.Id == p.UserId);
                if (user == null) continue;

                var permission = new Permission
                {
                    PermissionId = p.PermissionId,
                    PermissionValue = p.PermissionValue,
                    PermissionDisplayName = p.PermissionDisplayName,
                    PermissionSetId = p.PermissionSetId,
                    PermissionSetName = p.PermissionSetName,
                    ModuleId = p.ModuleId,
                    ModuleName = p.ModuleName,
                    ApiName = p.ApiName,
                    IsActive = p.IsActive
                };

                user.PermissionsDenied.Add(permission);
            }
        }

        private void LoadUserUiPermissions(List<UserDto> users, List<IdentityUserUiPermission> list)
        {
            foreach (var item in list)
            {
                var user = users.Find(u => u.Id == item.UserId);
                if (user == null) continue;

                var ui = new UserUiPermissionDto
                {
                    UserId = item.UserId,
                    UiPermission = new UiPermission
                    {
                        PermissionId = item.PermissionId,
                        PermissionValue = item.PermissionValue,
                        PermissionDisplayName = item.PermissionDisplayName,
                        ModuleId = item.ModuleId,
                        ModuleName = item.ModuleName,
                        PermissionTypeId = item.PermissionTypeId,
                        PermissionTypeName = item.PermissionTypeName,
                        PermissionParentId = item.PermissionParentId,
                        PermissionParentName = item.PermissionParentName,
                        IsActive = item.IsActive
                    }
                };

                user.UserUiPermissions.Add(ui);
            }
        }

        private async Task LoadUserRolePermissionsAsync(List<UserDto> users)
        {
            foreach (var user in users)
            {
                foreach (var role in user.Roles)
                {
                    var roleData = await GetPermissionsForRoleAsync(role.Id);

                    if (roleData != null)
                    {
                        user.UserPermissions.AddRange(roleData.RolePermissions);
                    }
                }
            }
        }
        public async Task<RoleDto> GetPermissionsForRoleAsync(string roleId)
        {
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UserDataAccess.PermissionsForRoleAsync - In process");

                IEnumerable<Permission> rolePermission = Enumerable.Empty<Permission>();

                string query = _sqlQueries["RolePermissionInfo.Select"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Execute query with Dapper's QueryMultiple to get multiple result sets
                    rolePermission = await _dapperDbConnection.QueryAsync<NetAuth.Domain.Entities.Permission>(
                    query,
                    new { RoleId = roleId }
                );
                }

                var role = new RoleDto();
                role.Id = roleId;

                role.RolePermissions = rolePermission.ToList();



                //Step 5 : Logging Completed
                _logger.LogInformation("IdentityManager.GetPermissionsForRoleAsync - Completed");

                //Step 6 : Return role & its permissions
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserDataAccess.PermissionsForRoleAsync - " + ex.Message);
                throw;
            }
        }

        #endregion
    }
}
