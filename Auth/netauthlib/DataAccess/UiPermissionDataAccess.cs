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
    /// Data Access layer :where we write code to connect DB and fetch or manipulate records from DB.
    /// In the database layer, we'll find things like database, connection, table, SQL, and result set.
    /// </summary>
    internal class UiPermissionDataAccess : IUiPermissionDataAccess
    {
        private ILogger<UiPermissionDataAccess> _logger;
        private IConfiguration _configuration;
        private readonly Dictionary<string, string> _sqlQueries;
        private readonly IConnectionHelper _connectionHelper;

        /// <summary>
        /// Instantiation of UiPermissionDataAccess class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public UiPermissionDataAccess(IConfiguration configuration, ILogger<UiPermissionDataAccess> logger, IConnectionHelper connectionHelper)
        {
            this._logger = logger;
            this._configuration = configuration;

            this._connectionHelper = connectionHelper;
            this._sqlQueries = _connectionHelper.LoadSqlQueriesXml("Auth");
        }


        /// <summary>
        /// Get UiPermissions For Role
        /// </summary>
        /// <returns></returns>
        public async Task<List<NetAuth.Domain.Dto.RoleUiPermissionDto>> GetUiPermissionsForRole(string roleId)
        {
            try
            {

                List<RoleUiPermissionDto> roleUiPermissions = new List<RoleUiPermissionDto>();
                List<IdentityUserUiPermission> uiPermissions = new List<IdentityUserUiPermission>();

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    uiPermissions = (await _dapperDbConnection.QueryAsync<IdentityUserUiPermission>(_sqlQueries["RoleUiPermissionsInfo.Select"], new { RoleId = roleId })).ToList();

                }

                foreach (var uiPermission in uiPermissions)
                {
                    var roleUiPermission = new RoleUiPermissionDto();
                    roleUiPermission.RoleId = roleId;

                    roleUiPermission.UiPermission = new UiPermission();
                    roleUiPermission.UiPermission.PermissionId = uiPermission.PermissionId;
                    roleUiPermission.UiPermission.PermissionDisplayName = uiPermission.PermissionDisplayName;
                    roleUiPermission.UiPermission.PermissionValue = uiPermission.PermissionValue;
                    roleUiPermission.UiPermission.PermissionTypeId = uiPermission.PermissionTypeId;
                    roleUiPermission.UiPermission.PermissionTypeName = uiPermission.PermissionTypeName;
                    roleUiPermission.UiPermission.ModuleId = uiPermission.ModuleId;
                    roleUiPermission.UiPermission.ModuleName = uiPermission.ModuleName;
                    roleUiPermission.UiPermission.PermissionParentId = uiPermission.PermissionParentId;
                    roleUiPermission.UiPermission.PermissionParentName = uiPermission.PermissionParentName;

                    roleUiPermissions.Add(roleUiPermission);

                }
                //Step 4 : Return roleUiPermissions
                return roleUiPermissions;
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
        /// <returns></returns>
        public async Task<List<NetAuth.Domain.Entities.UiPermission>> GetUiPermissions()
        {
            try
            {
                List<NetAuth.Domain.Entities.UiPermission> allUiPermissionsList = new List<NetAuth.Domain.Entities.UiPermission>();
                //Step 1: Logging Information
                _logger.LogInformation("UiPermissionDataAccess.GetUiPermissions - In process");


                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    //Step 2: Execute Reader
                    allUiPermissionsList = (await _dapperDbConnection.QueryAsync<NetAuth.Domain.Entities.UiPermission>(_sqlQueries["UIPermission.Select"])).ToList();
                }
                //Step 3: Logging Information Completed
                _logger.LogInformation("UiPermissionDataAccess.GetUiPermissions - Completed");

                //Step 4: Return UI permissions
                return allUiPermissionsList;
            }
            catch (Exception ex)
            {
                _logger.LogError("UiPermissionDataAccess.GetUiPermissions - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Add UiPermission
        /// </summary>
        /// <param name="UiPermission"></param>
        /// <param name="userName"></param>
        /// <returns>bool</returns>
        public async Task<string> AddUiPermission(UiPermission UiPermission)
        {
            string insertedId = string.Empty;
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UiPermissionDataAccess.AddUiPermission - In process");

                var existingUIPermissions = await GetUiPermissions();

                var IsUiPermissionExists = existingUIPermissions.Exists(x => x.PermissionValue.Equals(UiPermission.PermissionValue)
                                                        && x.PermissionParentId == UiPermission.PermissionParentId);

                if (!IsUiPermissionExists)
                {
                    var parameters = new
                    {
                        PermissionValue = UiPermission.PermissionValue,
                        PermissionDisplayName = UiPermission.PermissionDisplayName,
                        PermissionTypeId = UiPermission.PermissionTypeId,
                        PermissionParentId = UiPermission.PermissionParentId,
                        ModuleId = UiPermission.ModuleId,
                        IsAuthorized = (bool?)null,
                        OwnerId = (string?)null,
                        SysData = (string?)null,
                        TenantId = (string?)null,
                        SubTenantId = (string?)null,
                        CreatedBy = UiPermission.CreatedBy
                    };

                    using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                    {
                        // Step 3: Execute and return Id
                        insertedId = await _dapperDbConnection.ExecuteScalarAsync<string>(_sqlQueries["UIPermission.Save"], parameters);
                        UiPermission.PermissionId = insertedId;
                    }
                }

                //Step 5: Logging Information
                _logger.LogInformation("UiPermissionDataAccess.AddUiPermission - Completed");

                //Step 6: Return insertedId 
                return insertedId;
            }
            catch (Exception ex)
            {
                _logger.LogError("UiPermissionDataAccess.AddUiPermission - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Activate UiPermission
        /// </summary>
        /// <param name="UiPermission"></param>
        /// <returns></returns>
        public async Task<bool> ActivateUiPermission(UiPermission UiPermission)
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UiPermissionDataAccess.ActivateUiPermission - In process");

                //Step 2 :Assigning values to  Parameters

                var parameters = new
                {
                    IsActive = UiPermission.IsActive,
                    PermissionDisplayName = UiPermission.PermissionDisplayName,
                    UpdatedBy = UiPermission.UpdatedBy,
                    UpdatedDateTime = DateTime.UtcNow,
                    PermissionId = UiPermission.PermissionId
                };

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    await _dapperDbConnection.ExecuteAsync(_sqlQueries["UIPermission.Activate"], parameters);
                    retval = true;

                }

                ////Step 5: Dispatch Events
                //await DispatchEvents(new NetAuth.Domain.Entities.User());

                //Step 6: Logging Information
                _logger.LogInformation("UiPermissionDataAccess.ActivateUiPermission - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UiPermissionDataAccess.ActivateUiPermission - " + ex.Message);
                throw;
            }

            //Step 7: Return retval 
            return retval;
        }

        /// <summary>
        /// Add UiPermissions For Role
        /// </summary>
        /// <param name="uiPermissionsForRoles"></param>
        /// <returns></returns>
        public async Task<bool> AddUiPermissionsForRole(List<RoleUiPermissionDto> uiPermissionsForRoles)
        {
            bool retVal = false;
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("UiPermissionDataAccess.AddUiPermissionsForRole - In process");

                string deleteQuery = _sqlQueries["RoleUIPermission.Delete"];
                var roleId = uiPermissionsForRoles.FirstOrDefault()?.RoleId;

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    await _dapperDbConnection.ExecuteAsync(deleteQuery, new { RoleId = roleId });

                    foreach (RoleUiPermissionDto uiPermission in uiPermissionsForRoles)
                    {
                        await AddUiPermissionForRole(uiPermission, _dapperDbConnection);
                    }
                }
                //Step 3: Logging Information
                _logger.LogInformation("UiPermissionDataAccess.AddUiPermissionsForRole - Completed");

            }
            catch (Exception ex)
            {
                _logger.LogError("UiPermissionDataAccess.AddUiPermissionsForRole - " + ex.Message);
                throw;
            }

            //Step 7: Return retval 
            return retVal;
        }

        /// <summary>
        /// Add UiPermission For Role
        /// </summary>
        /// <param name="roleUiPermission"></param>
        /// <returns></returns>
        private async Task<string> AddUiPermissionForRole(RoleUiPermissionDto roleUiPermission, IDbConnection _dapperDbConnection)
        {
            string insertedId = string.Empty;
            try
            {
                _logger.LogInformation("AddUiPermissionForRole - In process");

                var parameters = new
                {
                    roleUiPermission.RoleId,
                    UiPermissionId = roleUiPermission.UiPermission.PermissionId,
                    roleUiPermission.CreatedBy
                };

                insertedId = await _dapperDbConnection.ExecuteScalarAsync<string>(_sqlQueries["RoleUIPermission.Save"], parameters);



                _logger.LogInformation("AddUiPermissionForRole - Completed");

                return insertedId;
            }
            catch (Exception ex)
            {
                _logger.LogError("UiPermissionDataAccess.AddUiPermissionForRole - " + ex.Message);
                throw;
            }
        }
    }
}




