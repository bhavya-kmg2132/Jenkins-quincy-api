using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DataAccess
{
    /// <summary>
    /// Data Access layer :where we write code to connect DB and fetch or manipulate records from DB.
    /// In the database layer, we'll find things like database, connection, table, SQL, and result set.
    /// </summary>
    public class InitialSetUpDataAccess : IInitialSetUpDataAccess
    {
        private ILogger<InitialSetUpDataAccess> _logger;
        private IConfiguration _configuration;
        private readonly Dictionary<string, string> _sqlQueries;
        private readonly IConnectionHelper _connectionHelper;
        /// <summary>
        /// Instantiation of InitialSetUpDataAccess class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public InitialSetUpDataAccess(IConfiguration configuration, ILogger<InitialSetUpDataAccess> logger, IConnectionHelper connectionHelper)
        {
            this._logger = logger;
            this._configuration = configuration;
            _connectionHelper = connectionHelper;
            _sqlQueries = _connectionHelper.LoadSqlQueriesXml("InitialSetup");
        }

        #region Public methods

        /// <summary>
        /// Add all the Initial setup Tables
        /// </summary>
        /// <returns>bool</returns>
        public async Task<bool> Add()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information In process
                _logger.LogInformation("InitialSetUpDataAccess.Add - In process");

                //Step 2: Save Initial SetUp Tables


                await AddAuthReferenceLookup();

                await AddRoles();
                await AddUIPermissions();

                await AddPermission();
                await AddUserAccessLevelMaster();

                await AddUser();
                await AddAppUser();

                await AddUserProfile();
                await AddUserPasswordHash();
                await AddUserActivity();

                await AddRolePermissions();
                await AddRoleUIPermissions();

                await AddUserRole();

                await AddPermissionGranted();
                await AddPermissionDenied();

                await AddUIPermissionGranted();
                await AddUIPermissionDenied();


                //Step 3: Set retval= true, if all above methods exec
                retval = true;

                //Step 4: Logging Information 
                _logger.LogInformation("InitialSetUpDataAccess.Add - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.Add - " + ex.Message);
                throw;
            }

            //Step 5: Return retval
            return retval;
        }

        #endregion


        #region Private methods 



        /// <summary>
        ///   Permission
        /// </summary>
        /// <returns>bool</returns>
        private async Task<bool> AddPermission()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddPermission - In process");

                //Step 2: Query to Create & Insert
                var query = _sqlQueries["InitialSetup.AddPermission"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

                // Step 4: Logging Information
                _logger.LogInformation("InitialSetUpDataAccess.AddPermission - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddPermission - " + ex.Message);
                throw;
            }

            return retval;
        }

        /// <summary>
        ///  PermissionDenied
        /// </summary>
        /// <returns>bool</returns>
        private async Task<bool> AddPermissionDenied()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddPermissionDenied - In process");

                var query = _sqlQueries["InitialSetup.AddPermissionDenied"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddPermissionDenied - " + ex.Message);
                throw;
            }

            return retval;
        }



        /// <summary>
        /// Role
        /// </summary>
        /// <returns>bool</returns>
        private async Task<bool> AddRoles()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddRoles - In process");

                // Step 2: Retrieve SQL Query
                var query = _sqlQueries["InitialSetup.AddRole"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddRoles - " + ex.Message);
                throw;
            }

            return retval;
        }

        /// <summary>
        ///  RolePermissions
        /// </summary>
        /// <returns>bool</returns>
        private async Task<bool> AddRolePermissions()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddRolePermissions - In process");

                //Step 2: Query to Create & Insert
                var query = _sqlQueries["InitialSetup.AddRolePermission"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddRolesPermission - " + ex.Message);
                throw;
            }

            return retval;
        }

        /// <summary>
        ///  UserAccessLevel
        /// </summary>
        /// <returns>bool</returns>
        private async Task<bool> AddUserAccessLevelMaster()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddUserAccessLevelMaster - In process");

                var query = _sqlQueries["InitialSetup.AddUserAccessLevel"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddUserAccessLevelMaster - " + ex.Message);
                throw;
            }

            return retval;
        }

        /// <summary>
        ///  User
        /// </summary>
        /// <returns>bool</returns>
        private async Task<bool> AddUser()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddUser - In process");

                // Step 2: Retrieve SQL Query
                var query = _sqlQueries["InitialSetup.AddUser"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddUser - " + ex.Message);
                throw;
            }

            return retval;
        }


        /// <summary>
        /// UserRole
        /// </summary>
        /// <returns>bool</returns>
        private async Task<bool> AddUserRole()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddUserRole - In process");


                // Step 2: Retrieve SQL Query
                var query = _sqlQueries["InitialSetup.AddUserRole"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddUserRole - " + ex.Message);
                throw;
            }

            return retval;
        }

        /// <summary>
        ///  UserProfie
        /// </summary>
        /// <returns>bool</returns>
        private async Task<bool> AddUserProfile()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddUserProfile - In process");

                // Step 2: Retrieve SQL Query
                var query = _sqlQueries["InitialSetup.AddUserProfile"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddUserProfile - " + ex.Message);
                throw;
            }
            return retval;
        }


        public async Task<bool> AddAppUser()
        {
            bool retval = false;
            try
            {
                // Step 1: Logging Information
                _logger.LogInformation("InitialSetUpDataAccess.AddAppUser - In process");

                // Step 2: Retrieve SQL Query
                var query = _sqlQueries["InitialSetup.AppUser"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

                // Step 4: Logging Information
                _logger.LogInformation("InitialSetUpDataAccess.AddAppUser - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddAppUser - " + ex.Message);
                throw;
            }

            return retval;
        }



        public async Task<bool> AddAuthReferenceLookup()
        {
            bool retval = false;
            try
            {
                // Step 1: Logging Information
                _logger.LogInformation("InitialSetUpDataAccess.AddAuthReferenceLookup - In process");

                // Step 2: Retrieve SQL Query
                var query = _sqlQueries["InitialSetup.AddAuthReferenceLookup"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

                // Step 4: Logging Information
                _logger.LogInformation("InitialSetUpDataAccess.AddAuthReferenceLookup - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddAuthReferenceLookup - " + ex.Message);
                throw;
            }

            return retval;
        }

        private async Task<bool> AddPermissionGranted()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddPermissionGranted - In process");

                //Step 2: Query to Create & Insert
                var query = _sqlQueries["InitialSetup.AddPermissionGranted"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

                // Step 4: Logging Information
                _logger.LogInformation("InitialSetUpDataAccess.AddPermissionGranted - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddPermissionGranted - " + ex.Message);
                throw;
            }

            return retval;
        }

        private async Task<bool> AddUserActivity()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddUser - In process");

                // Step 2: Retrieve SQL Query
                var query = _sqlQueries["InitialSetup.AddUserActivity"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddUser - " + ex.Message);
                throw;
            }

            return retval;
        }


        private async Task<bool> AddUserPasswordHash()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddUserPasswordHash - In process");

                // Step 2: Retrieve SQL Query
                var query = _sqlQueries["InitialSetup.AddUserPasswordHash"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddUserPasswordHash - " + ex.Message);
                throw;
            }

            return retval;
        }

        private async Task<bool> AddRoleUIPermissions()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddRoleUiPermissions - In process");

                //Step 2: Query to Create & Insert
                var query = _sqlQueries["InitialSetup.AddRoleUiPermission"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddRolesUiPermission - " + ex.Message);
                throw;
            }

            return retval;
        }


        private async Task<bool> AddUIPermissions()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddUiPermissions - In process");

                //Step 2: Query to Create & Insert
                var query = _sqlQueries["InitialSetup.AddUiPermission"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddUiPermission - " + ex.Message);
                throw;
            }

            return retval;
        }


        private async Task<bool> AddUIPermissionDenied()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddUiPermissionDenied - In process");

                //Step 2: Query to Create & Insert
                var query = _sqlQueries["InitialSetup.AddUiPermissionDenied"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddUiPermissionDenied - " + ex.Message);
                throw;
            }

            return retval;
        }

        private async Task<bool> AddUIPermissionGranted()
        {
            bool retval = false;
            try
            {
                //Step 1: Logging Information : In process
                _logger.LogInformation("InitialSetUpDataAccess.AddUiPermissionGranted - In process");

                //Step 2: Query to Create & Insert
                var query = _sqlQueries["InitialSetup.AddUiPermissionGranted"];

                using (var _dapperDbConnection = _connectionHelper.CreateNetAuthConnection())
                {
                    // Step 3: Execute query
                    await _dapperDbConnection.ExecuteAsync(query);
                    retval = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("InitialSetUpDataAccess.AddUiPermissionGranted - " + ex.Message);
                throw;
            }

            return retval;
        }
        #endregion
    }
}



