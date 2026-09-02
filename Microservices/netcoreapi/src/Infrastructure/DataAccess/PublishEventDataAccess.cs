using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.PublishEvent.Queries;
using Dapper;
using Domain.Common;
using Domain.Enums;
using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Infrastructure.DataAccess
{
    /// <summary>
    /// Data Access layer :where we write code to connect DB and fetch or manipulate records from DB.
    /// In the database layer, we'll find things like database, connection, table, SQL, and result set.
    /// </summary>
    public class PublishEventDataAccess : IPublishEventDataAccess
    {
        private ILogger<PublishEventDataAccess> _logger;
        private IConfiguration _configuration;
        private readonly IConnectionHelper _connectionHelper;
        private readonly ICrmMasterDataAccess _crmMasterDataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly Dictionary<string, string> _sqlQueries;

        /// <summary>
        /// PublishEventDataAccess : Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>

        public PublishEventDataAccess(IConfiguration configuration, IConnectionHelper connectionHelper, ILogger<PublishEventDataAccess> logger, ICrmMasterDataAccess crmMasterDataAccess, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._connectionHelper = connectionHelper;
            _crmMasterDataAccess = crmMasterDataAccess;
            _currentUserService = currentUserService;
            _sqlQueries = _connectionHelper.LoadSqlQueriesXml("PublishEventData", DbConfigKeys.EventDb);

        }

        #region PublishEventDataAccess

        /// <summary>
        /// Add PublishEventData
        /// </summary>
        /// <returns>string</returns>

        public async Task<string> Add(PublishEventData PublishEventData)
        {
            try
            {
                _logger.LogInformation("PublishEventDataAccess.Add - In process");

                //Step 2: Serialize the PublishEventData object
                string serializedObjectForEventData = JsonSerializer.Serialize(PublishEventData.EventData,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        ReferenceHandler = ReferenceHandler.Preserve
                    });

                byte[] serializedObjectForEventDataBinary = MessagePackSerializer.Serialize(PublishEventData.EventData,
                                                      MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block));

                //Step 3: Assign values to Sql Parameters

                //var sqlParameters = new
                //{
                //    Id = Guid.NewGuid().ToString(),
                //    AuditableSourceEventName = PublishEventData.AuditableSourceEventName,
                //    OperationType = PublishEventData.OperationType,
                //    OperationDateTimeUtc = PublishEventData.CreatedDateTime,
                //    ApiName = PublishEventData.ApiName,
                //    CollectionName = PublishEventData.CollectionName,
                //    Data = serializedObjectForEventData
                //};

                var sqlParameters = new
                {
                    Id = Guid.NewGuid().ToString(),
                    CorrelationId = PublishEventData.CorrelationId,
                    AuditableRequestId = PublishEventData.AuditableRequestId,
                    AuditableRequestName = PublishEventData.AuditableRequestName,
                    AuditableAssemblyQualifiedName = PublishEventData.AuditableAssemblyQualifiedName,
                    AuditableSourceEventName = PublishEventData.AuditableSourceEventName,
                    CreatedDateTime = PublishEventData.CreatedDateTime,
                    ApiName = PublishEventData.ApiName,
                    CollectionName = PublishEventData.CollectionName,
                    EventData = serializedObjectForEventData,
                    EventdataBinary = serializedObjectForEventDataBinary,
                    UserId = _currentUserService.UserId,
                    OperationType = PublishEventData.OperationType
                };

                string insertedID = string.Empty;

                using (var _dapperDbConnection = _connectionHelper.CreateEventConnection())
                {
                    //Step 4: Execute Scalar method to add PublisEventData to db 
                    insertedID = await _dapperDbConnection.ExecuteScalarAsync<string>(_sqlQueries["PublishEventData.Add"], sqlParameters);
                }
                //Step 5: Logging Information
                _logger.LogInformation("PublishEventDataAccess.Add - Completed");

                //Step 6: Return insertedID 
                return insertedID;
            }
            catch (Exception ex)
            {
                _logger.LogError("PublishEventDataAccess.Add - " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// GetPublishEventDatas
        /// </summary>
        /// <returns>PublishEventData list</returns>
        public async Task<(List<PublishEventDataDto>, int)> GetList(int pageNumber, int pageSize, string orderType, string columnName, string filtersJson, string searchText)
        {
            List<PublishEventDataDto> PublishEventDatas = new List<PublishEventDataDto>();

            try
            {
                _logger.LogInformation("PublishEventDataDataAccess.GetList - In process");


                //columnName = string.IsNullOrEmpty(columnName) ? "UpdatedDateTime" : columnName;
                columnName = CrmMasterDataAccess.ColumnMapping.PublishEventDataColumnList.ContainsKey(columnName)
                 ? CrmMasterDataAccess.ColumnMapping.PublishEventDataColumnList[columnName]
                 : "\"Es\".\"CreatedDateTime\"";

                orderType = string.IsNullOrEmpty(orderType) ? "Desc" : orderType;
                pageNumber = pageNumber == 0 ? 1 : pageNumber;
                pageSize = pageSize == 0 ? 1000 : pageSize;

                JObject filterJsonObject = null;

                string joinClause = string.Empty;
                string whereClause = string.Empty;

                if (!string.IsNullOrEmpty(filtersJson) || !string.IsNullOrEmpty(searchText))
                {
                    if (!string.IsNullOrEmpty(filtersJson))
                        filterJsonObject = JObject.Parse(filtersJson);// Deserialize the string to JObject
                    var (joinClauses, whereClauses) = await _crmMasterDataAccess.BuildWhereClause(filterJsonObject, "PublishEventData", searchText);
                    joinClause = string.IsNullOrEmpty(joinClauses) ? string.Empty : joinClauses;
                    whereClause = string.IsNullOrEmpty(whereClauses) ? string.Empty : whereClauses;
                }


                var queryParams = new
                {
                    @PageSize = pageSize,
                    @PageNumber = pageNumber,
                    @ColumnName = columnName,
                    @OrderType = orderType,
                };
                string combinedQuery = $@"
                                        {string.Format(_sqlQueries["PublishEventData.GetList"], columnName, orderType)};
                                        {_sqlQueries["PublishEventData.GetPublishEventDataCount"]}";

                combinedQuery = combinedQuery.Replace("#JOIN_CLAUSE#", joinClause)
                                             .Replace("#WHERE_CLAUSE#", whereClause);

                using (var _dapperDbConnection = _connectionHelper.CreateEventConnection())
                {
                    using (var multi = await _dapperDbConnection.QueryMultipleAsync(combinedQuery, queryParams))
                    {
                        var result = (await multi.ReadAsync<PublishEventDataDto>()).ToList();

                        foreach (var item in result)
                        {
                            try
                            {
                                //item.EventDataJson = JsonSerializer.Deserialize<List<Property>>(item.EventData, new JsonSerializerOptions
                                //{
                                //    PropertyNameCaseInsensitive = true,
                                //    ReferenceHandler = ReferenceHandler.Preserve
                                //});

                                item.EventDataJson = (item.EventDataBinary != null && item.EventDataBinary.Length > 0) ?
                                                      MessagePackSerializer.Deserialize<List<Property>>(item.EventDataBinary,
                                                      MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block)) : null;
                            }
                            catch (System.Text.Json.JsonException ex)
                            {
                                _logger.LogWarning($"Invalid JSON in EventData. MessageId: {item.Id}, Error: {ex.Message}");
                            }
                        }
                        int totalCount = await multi.ReadFirstOrDefaultAsync<int>();

                        //// Populate Owner field
                        //foreach (var Record in result)
                        //{
                        //    Record.Owner = await _userDataAccess.GetUserFullNameByIdAsync(Record.OwnerId);
                        //    Record.Tags = Record?.TagsRaw?.Split(",").ToList();
                        //}

                        _logger.LogInformation("PublishEventDataDataAccess.GetList - All PublishEventDatas Fetched");
                        return (result, totalCount);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"PublishEventDataDataAccess.GetList - {ex.Message}", ex); // Improved error logging
                throw;
            }
        }

        #endregion
    }
}

