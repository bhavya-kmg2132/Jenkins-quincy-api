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
    public class EventDbInitialSetUpDataAccess : IEventDbInitialSetUpDataAccess
    {
        private readonly ILogger<EventDbInitialSetUpDataAccess> _logger;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _sqlQueries;
        private readonly IConnectionHelper _connectionHelper;

        public EventDbInitialSetUpDataAccess(IConfiguration configuration, ILogger<EventDbInitialSetUpDataAccess> logger, IConnectionHelper connectionHelper)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionHelper = connectionHelper;
            _sqlQueries = _connectionHelper.LoadSqlQueriesXml("EventDbInitialSetup", DbConfigKeys.EventDb);
        }

        #region Public methods

        public async Task<bool> Add()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("EventDbInitialSetUpDataAccess.Add - In process");

                await CreateEventStoreQuincy();
                await CreateFailedMassTransitMessage();
                await CreateNotificationRule();
                await CreateInSystemNotification();
                await CreateNotificationUserMapping();
                await CreateNotificationUserSubscription();
                await CreateNotificationRequest();
                await CreateNotificationResponse();

                retval = true;

                _logger.LogInformation("EventDbInitialSetUpDataAccess.Add - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("EventDbInitialSetUpDataAccess.Add - {Message}", ex.Message);
                throw;
            }

            return retval;
        }

        #endregion

        #region Private methods

        private async Task<bool> CreateEventStoreQuincy()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateEventStoreQuincy - In process");
                var query = _sqlQueries["EventDbInitialSetup.CreateEventStoreQuincy"];
                using (var conn = _connectionHelper.CreateEventConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateEventStoreQuincy - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("EventDbInitialSetUpDataAccess.CreateEventStoreQuincy - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        private async Task<bool> CreateFailedMassTransitMessage()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateFailedMassTransitMessage - In process");
                var query = _sqlQueries["EventDbInitialSetup.CreateFailedMassTransitMessage"];
                using (var conn = _connectionHelper.CreateEventConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateFailedMassTransitMessage - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("EventDbInitialSetUpDataAccess.CreateFailedMassTransitMessage - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        private async Task<bool> CreateNotificationRule()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateNotificationRule - In process");
                var query = _sqlQueries["EventDbInitialSetup.CreateNotificationRule"];
                using (var conn = _connectionHelper.CreateEventConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateNotificationRule - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("EventDbInitialSetUpDataAccess.CreateNotificationRule - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        private async Task<bool> CreateInSystemNotification()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateInSystemNotification - In process");
                var query = _sqlQueries["EventDbInitialSetup.CreateInSystemNotification"];
                using (var conn = _connectionHelper.CreateEventConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateInSystemNotification - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("EventDbInitialSetUpDataAccess.CreateInSystemNotification - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        private async Task<bool> CreateNotificationUserMapping()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateNotificationUserMapping - In process");
                var query = _sqlQueries["EventDbInitialSetup.CreateNotificationUserMapping"];
                using (var conn = _connectionHelper.CreateEventConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateNotificationUserMapping - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("EventDbInitialSetUpDataAccess.CreateNotificationUserMapping - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        private async Task<bool> CreateNotificationUserSubscription()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateNotificationUserSubscription - In process");
                var query = _sqlQueries["EventDbInitialSetup.CreateNotificationUserSubscription"];
                using (var conn = _connectionHelper.CreateEventConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateNotificationUserSubscription - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("EventDbInitialSetUpDataAccess.CreateNotificationUserSubscription - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        private async Task<bool> CreateNotificationRequest()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateNotificationRequest - In process");
                var query = _sqlQueries["EventDbInitialSetup.CreateNotificationRequest"];
                using (var conn = _connectionHelper.CreateEventConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateNotificationRequest - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("EventDbInitialSetUpDataAccess.CreateNotificationRequest - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        private async Task<bool> CreateNotificationResponse()
        {
            bool retval = false;
            try
            {
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateNotificationResponse - In process");
                var query = _sqlQueries["EventDbInitialSetup.CreateNotificationResponse"];
                using (var conn = _connectionHelper.CreateEventConnection())
                {
                    await conn.ExecuteAsync(query);
                    retval = true;
                }
                _logger.LogInformation("EventDbInitialSetUpDataAccess.CreateNotificationResponse - Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError("EventDbInitialSetUpDataAccess.CreateNotificationResponse - {Message}", ex.Message);
                throw;
            }
            return retval;
        }

        #endregion
    }
}
