using Application.Common.Interfaces;
using DataAccess.Common;
using Dapper;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess
{
    public class MainDbInitialSetUpDataAccess : IMainDbInitialSetUpDataAccess
    {
        private readonly ILogger<MainDbInitialSetUpDataAccess> _logger;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _sqlQueries;
        private readonly IConnectionHelper _connectionHelper;

        public MainDbInitialSetUpDataAccess(IConfiguration configuration, ILogger<MainDbInitialSetUpDataAccess> logger, IConnectionHelper connectionHelper)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionHelper = connectionHelper;
            _sqlQueries = _connectionHelper.LoadSqlQueriesXml("MainDbInitialSetup");
        }

        #region Public methods

        public async Task<bool> Add()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("MainDbInitialSetUpDataAccess.Add - In process");

                await CreateCache();
                await CreateDeletedInMemoryCacheLog();
                await CreateMcaPolicy();
                await CreateReferenceCustomField();
                await CreateSchemaVersions();
                await CreateVersionTrack();
                await CreateUiConfig();

                retval = true;

                _logger.LogInformation("MainDbInitialSetUpDataAccess.Add - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("MainDbInitialSetUpDataAccess.Add - {Message}", ex.Message);
                throw;
            }

            return retval;
        }

        #endregion

        #region Private methods

        private async Task<bool> CreateCache()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("MainDbInitialSetUpDataAccess.CreateCache - In process");
                var query = _sqlQueries["MainDbInitialSetup.CreateCache"];
                using (var conn = _connectionHelper.CreateConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("MainDbInitialSetUpDataAccess.CreateCache - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("MainDbInitialSetUpDataAccess.CreateCache - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        private async Task<bool> CreateDeletedInMemoryCacheLog()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("MainDbInitialSetUpDataAccess.CreateDeletedInMemoryCacheLog - In process");
                var query = _sqlQueries["MainDbInitialSetup.CreateDeletedInMemoryCacheLog"];
                using (var conn = _connectionHelper.CreateConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("MainDbInitialSetUpDataAccess.CreateDeletedInMemoryCacheLog - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("MainDbInitialSetUpDataAccess.CreateDeletedInMemoryCacheLog - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        private async Task<bool> CreateMcaPolicy()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("MainDbInitialSetUpDataAccess.CreateMcaPolicy - In process");
                var query = _sqlQueries["MainDbInitialSetup.CreateMcaPolicy"];
                using (var conn = _connectionHelper.CreateConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("MainDbInitialSetUpDataAccess.CreateMcaPolicy - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("MainDbInitialSetUpDataAccess.CreateMcaPolicy - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        private async Task<bool> CreateReferenceCustomField()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("MainDbInitialSetUpDataAccess.CreateReferenceCustomField - In process");
                var query = _sqlQueries["MainDbInitialSetup.CreateReferenceCustomField"];
                using (var conn = _connectionHelper.CreateConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("MainDbInitialSetUpDataAccess.CreateReferenceCustomField - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("MainDbInitialSetUpDataAccess.CreateReferenceCustomField - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        private async Task<bool> CreateSchemaVersions()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("MainDbInitialSetUpDataAccess.CreateSchemaVersions - In process");
                var query = _sqlQueries["MainDbInitialSetup.CreateSchemaVersions"];
                using (var conn = _connectionHelper.CreateConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("MainDbInitialSetUpDataAccess.CreateSchemaVersions - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("MainDbInitialSetUpDataAccess.CreateSchemaVersions - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        private async Task<bool> CreateVersionTrack()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("MainDbInitialSetUpDataAccess.CreateVersionTrack - In process");
                var query = _sqlQueries["MainDbInitialSetup.CreateVersionTrack"];
                using (var conn = _connectionHelper.CreateConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("MainDbInitialSetUpDataAccess.CreateVersionTrack - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("MainDbInitialSetUpDataAccess.CreateVersionTrack - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        private async Task<bool> CreateUiConfig()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("MainDbInitialSetUpDataAccess.CreateUiConfig - In process");
                var query = _sqlQueries["MainDbInitialSetup.CreateUiConfig"];
                using (var conn = _connectionHelper.CreateConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("MainDbInitialSetUpDataAccess.CreateUiConfig - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("MainDbInitialSetUpDataAccess.CreateUiConfig - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        #endregion
    }
}
