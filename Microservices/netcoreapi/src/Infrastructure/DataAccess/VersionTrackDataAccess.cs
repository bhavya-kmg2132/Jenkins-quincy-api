using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Dapper;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DataAccess
{
    public class VersionTrackDataAccess : IVersionTrackDataAccess
    {
        private ILogger<VersionTrackDataAccess> _logger;
        private IConfiguration _configuration;
        private readonly IDomainEventService _domainEventService;
        private readonly IMasterDataAccess _masterDataAccess;

        private readonly Dictionary<string, string> _sqlQueries;
        private readonly IConnectionHelper _connectionHelper;
        /// <summary>
        /// VersionTrackDataAccess : Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="domainEventService"></param>
        public VersionTrackDataAccess(IConfiguration configuration, ILogger<VersionTrackDataAccess> logger, IDomainEventService domainEventService, IConnectionHelper connectionHelper, IMasterDataAccess masterDataAccess)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._domainEventService = domainEventService;

            this._connectionHelper = connectionHelper;
            this._sqlQueries = _connectionHelper.LoadSqlQueriesXml("VersionTrack");
            this._masterDataAccess = masterDataAccess;
        }

        /// <summary>
        /// Get VersionTrack List
        /// </summary>
        /// <returns>List<VersionTracks></VersionTracks></VersionTrack></returns>

        public async Task<List<Domain.Entities.VersionTrack>> GetVersionTrack()
        {

            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("VersionTrackDataAccess.GetVersionTrack - In process");

                //Step 2: Retrieve SQL Query 
                var queryTemplate = _sqlQueries["VersionTrack.GetVersionTrack"];

                using (var _dapperDbConnection = _connectionHelper.CreateConnection())
                {
                    //Step 3: Fetch data from DB using dapper
                    List<VersionTrack> versionTrackList = (List<VersionTrack>)await _dapperDbConnection.QueryAsync<VersionTrack>(queryTemplate);

                    //Step 4: Logging Information
                    _logger.LogInformation("VersionTrackDataAccess.GetVersionTrack - Completed");

                    //Step 5: Return acme
                    return versionTrackList;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"VersionTrackDataAccess.GetVersionTrack - {ex.Message}");
                throw;
            }
        }



        /// <summary>
        /// Add : Add VersionTrack
        /// </summary>
        /// <param name="VersionTrack"></param>
        /// <returns>string</returns>
        public async Task<string> Add(VersionTrack versionTrack)
        {
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("VersionTrackDataAccess.Add - In process");

                //Step 2: Retrieve SQL Query 
                var addQuery = _sqlQueries["VersionTrack.Add"];

                using (var _dapperDbConnection = _connectionHelper.CreateConnection())
                {
                    //Step 3: Add Data from the Database Using Dapper
                    string rowsAffected = await _dapperDbConnection.ExecuteScalarAsync<string>(addQuery, versionTrack);

                    //Step 4: Logging Information
                    _logger.LogInformation("VersionTrackDataAccess.Add - Completed");

                    //Step 5: Returns rowsAffected
                    return rowsAffected;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("VersionTrackDataAccess.Add - " + ex.Message);
                throw;
            }
        }

    }
}
