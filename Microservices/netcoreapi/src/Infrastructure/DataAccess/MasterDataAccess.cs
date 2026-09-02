using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Dapper;
using Dapper.Extensions;
using Domain.Common;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace Infrastructure.DataAccess
{
    /// <summary>
    ///
    /// </summary>
    public class MasterDataAccess : IMasterDataAccess
    {
        private ILogger<MasterDataAccess> _logger;
        private IConfiguration _configuration;
        private readonly IDomainEventService _domainEventService;
        private readonly Dictionary<string, string> _sqlQueries;
        public IDapper _dapper;
        private readonly IConnectionHelper _connectionHelper;

        private const string _cacheKeyForMaster = "MasterDataAccess|GetGenericMasterListSql";

        /// <summary>
        /// MasterDataAccess : Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="domainEventService"></param>
        public MasterDataAccess(IConfiguration configuration, ILogger<MasterDataAccess> logger, IDomainEventService domainEventService, IConnectionHelper connectionHelper)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._domainEventService = domainEventService;

            //SqlConnectionStringBuilder sqlconnectionbuilder = new SqlConnectionStringBuilder(this._configuration["ConnectionStrings:SqlDBConnection"]);
            this._connectionHelper = connectionHelper;
            this._sqlQueries = _connectionHelper.LoadSqlQueriesXml("Master");

        }

        /// <summary>
        /// GetReferenceCustomFields
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>GetReferenceCustomFields</returns>
        public async Task<ReferenceCustomFields> GetReferenceCustomFields(string tableName)
        {
            try
            {
                //Step 1: Logging Information
                _logger.LogInformation("MasterDataAccess.GetReferenceCustomFields - In process");

                //Step 2: Retrieve SQL Query 
                var getQuery = _sqlQueries["Master.GetReferenceCustomFields"];

                //Step 3: Declare queryPraram
                var queryParams = new { TableName = tableName };

                using (var _dapperDbConnection = _connectionHelper.CreateConnection())
                {
                    //Step 4: Fetch data from DB using dapper
                    var customFieldsJson = await _dapperDbConnection.ExecuteScalarAsync<string>(getQuery, queryParams);


                    //Step 5: Deserialize JSON into List<CustomField>
                    var referenceCustomFields = new ReferenceCustomFields
                    {
                        CustomFields = !string.IsNullOrEmpty(customFieldsJson)
                            ? JsonSerializer.Deserialize<List<CustomField>>(customFieldsJson)
                            : new List<CustomField>()
                    };

                    //Step 6: Logging Information
                    _logger.LogInformation("MasterDataAccess.GetReferenceCustomFields - Completed");

                    // Step 7: Return ReferenceCustomFields
                    return referenceCustomFields;
                }
            }
            catch (Exception ex)
            {
                //Step 8: Error Handling: Log & Rethrow Exception
                _logger.LogError("PolicyDataAccess.GetReferenceCustomFieldsByTableName - " + ex.Message);
                throw;
            }
        }

        public async Task<string> BuildWhereClause(JObject filters, string moduleName, string searchText)
        {
            var whereClauses = new List<string>();
            Dictionary<string, string> columnMappings = ColumnMapping.GetColumnList(moduleName);

            if (filters != null)
            {
                foreach (var filter in filters.Properties())
                {
                    var columnName = filter.Name;
                    var filterValue = filter.Value;

                    string conditionSql = null;

                    if (columnMappings.ContainsKey(columnName))
                    {
                        string column = columnMappings[columnName];

                        conditionSql = BuildCondition(column, filterValue);
                    }

                    if (!string.IsNullOrEmpty(conditionSql))
                    {
                        whereClauses.Add(conditionSql);
                    }
                }
            }
            if (!string.IsNullOrEmpty(searchText))
            {
                var searchConditions = new List<string>();

                Dictionary<string, string> columnSearchMappings = ColumnMapping.GetColumnList(moduleName, true);

                foreach (var column in columnSearchMappings.Values)
                {
                    searchConditions.Add($"({column} = '{searchText}' OR {column} LIKE '%{searchText}%')");
                }

                // Combine all search conditions with OR
                if (searchConditions.Any())
                {
                    whereClauses.Add($"({string.Join(" OR ", searchConditions)})");
                }
            }

            return (whereClauses.Count > 0 ? "AND " + string.Join(" AND ", whereClauses) : string.Empty);
        }

        /// <summary>
        /// BuildCondition
        /// </summary>
        /// <param name="column"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        private string BuildCondition(string column, JToken filterValue)
        {
            if (filterValue is JObject conditions)
            {
                var conditionStrings = new List<string>();

                // Check if conditions array is present
                if (conditions.ContainsKey("conditions"))
                {
                    var operatorType = conditions["operator"]?.ToString() ?? "AND"; // Default to AND if no operator
                    foreach (var condition in conditions["conditions"])
                    {
                        var filterType = condition.Value<string>("filterType");
                        var conditionString = BuildConditionByFilterType(column, filterType, condition);
                        if (!string.IsNullOrEmpty(conditionString))
                        {
                            conditionStrings.Add(conditionString);
                        }

                    }
                    // Combine all conditions with the operator (AND/OR)
                    return $"({string.Join($" {operatorType} ", conditionStrings)})";
                }
                else
                {
                    // Single condition handling
                    var filterType = conditions.Value<string>("filterType");
                    return BuildConditionByFilterType(column, filterType, conditions);
                }
            }
            else
            {
                // Handle simple conditions like "contains"
                return $"{column} LIKE '%{filterValue}%'";
            }

        }

        /// <summary>
        /// BuildConditionByFilterType : Based on filter type choose the prepare the where clause
        /// </summary>
        /// <param name="column"></param>
        /// <param name="filterType"></param>
        /// <param name="filterValue"></param>
        /// <returns>string</returns>
        private string BuildConditionByFilterType(string column, string filterType, JToken filterValue)
        {
            if (string.IsNullOrEmpty(filterType))
                return string.Empty;
            switch (filterType)
            {
                case "number":
                    return BuildNumberCondition(column, filterValue);
                case "text":
                    return BuildTextCondition(column, filterValue);
                case "date":
                    return BuildDateCondition(column, filterValue);
                case "set":
                    return BuildSetCondition(column, filterValue);
                default:
                    return $"{column} LIKE '%{filterValue.ToString()}%'";
            }
        }

        /// <summary>
        /// BuildNumberCondition : Build clause when filter type is number
        /// </summary>
        /// <param name="column"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string BuildNumberCondition(string column, JToken filterValue)
        {
            var conditionType = filterValue.Value<string>("type");
            var filter = filterValue.Value<string>("filter");
            var filterTo = filterValue.Value<string>("filterTo");
            return conditionType switch
            {
                "greaterThan" => $"{column} > {filter}",
                "greaterThanOrEqual" => $"{column} >= {filter}",
                "lessThan" => $"{column}<{filter}",
                "lessThanOrEqual" => $"{column} <= {filter}",
                "equals" => $"{column} ={filter}",
                "notEqual" => $"{column}<>{filter}",
                "inRange" => $"{column} BETWEEN {filter} AND {filterTo}",
                "blank" => $"{column} IS NULL",
                "notBlank" => $"{column} IS NOT NULL",
                _ => throw new ArgumentException($"Unsupported condition type: {conditionType}")
            };
        }

        /// <summary>
        /// BuildTextCondition : Build clause when filter type is text
        /// </summary>
        /// <param name="column"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string BuildTextCondition(string column, JToken filterValue)
        {
            var conditionType = filterValue.Value<string>("type");
            var filter = filterValue.Value<string>("filter");
            return conditionType switch
            {
                "contains" => $"{column} LIKE '%{filter}%'",
                "notContains" => $"{column} NOT LIKE '%{filter}%'",
                "equals" => $"{column} = '{filter}' ",
                "notEqual" => $"{column}<>'{filter}'",
                "startsWith" => $"{column} LIKE '{filter}%'",
                "endsWith" => $"{column} LIKE '%{filter}'",
                "blank" => $"{column} IS NULL",
                "notBlank" => $"{column} IS NOT NULL",
                _ => throw new ArgumentException($"Unsupported condition type: {conditionType}")
            };
        }

        /// <summary>
        /// BuildDateCondition : Build clause when filter type is date
        /// </summary>
        /// <param name="column"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string BuildDateCondition(string column, JToken filterValue)
        {
            var conditionType = filterValue.Value<string>("type");
            var filterTo = filterValue.Value<string>("dateTo");
            var filter = filterValue.Value<string>("dateFrom");
            return conditionType switch
            {
                "greaterThan" => $"{column} > '{filter}'",
                "lessThan" => $"{column}<'{filter}'",
                "equals" => $"{column} ='{filter}'",
                "notEqual" => $"{column}<>'{filter}'",
                "inRange" => $"{column} BETWEEN '{filter}' AND '{filterTo}'",
                "blank" => $"{column} IS NULL",
                "notBlank" => $"{column} IS NOT NULL",
                _ => throw new ArgumentException($"Unsupported condition type: {conditionType}")
            };
        }

        /// <summary>
        /// BuildSetCondition : Build clause when filter type is set
        /// </summary>
        /// <param name="column"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        private string BuildSetCondition(string column, JToken filterValue)
        {
            try
            {
                var conditionType = filterValue.Value<string>("filterType");
                var valuesToken = filterValue["values"];

                List<string> values;

                if (valuesToken is JArray array)  // If it's an array, extract values
                {
                    values = array.Select(f => $"'{f.ToString().Trim()}'").ToList();
                }
                else  // If it's a single string
                {
                    var filter = filterValue.Value<string>("values") ?? "";
                    values = filter.Split(',').Select(f => $"'{f.Trim()}'").ToList();
                }
                return $"{column} IN ({string.Join(",", values)})";
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// ColumnMapping : stores the column Mapping Dictionary
        /// </summary>
        public static class ColumnMapping
        {
            public static readonly Dictionary<string, string> TodoItemColumnList = new Dictionary<string, string>()
            {
                { "id", "Id" },
                { "listId", "ListId" },
                { "title", "Title" },
                { "note", "Note" },
                { "priority","Priority" },
                { "reminder", "Reminder" },
                { "createdBy","CreatedBy" },
                { "createdDateTime", "CONVERT(DATE, CreatedDateTime)" },
                { "updatedBy", "UpdatedBy" },
                { "updatedDateTime", "CONVERT(DATE, UpdatedDateTime)" },
                { "updateReason", "UpdateReason" },
                { "ownerId", "OwnerId" },
                { "ownerName", "OwnerName" },
                { "isActive", "IsActive" },
                { "isDeleted", "IsDeleted" },
                { "isApproved", "IsApproved" },
                { "approverId", "ApproverId" },
                { "approvedDateTime", "ApprovedDateTime" },
                { "isAuthorized", "IsAuthorized" },
                { "authorizedById", "AuthorizedById" },
                { "authorizedDateTime", "AuthorizedDateTime" },
                { "tenantId", "TenantId" },
                { "subTenantId", "SubTenantId" },
                { "sysData", "SysData" },
            };

            public static Dictionary<string, string> GetColumnList(string moduleName, bool isSearchColumn = false)
            {
                return (moduleName, isSearchColumn) switch
                {
                    ("TodoItem", false) => TodoItemColumnList,

                    _ => throw new ArgumentException($"Unknown module: {moduleName}")
                };
            }


        }

        public async Task<List<GenericMasterList>> GetFilterGenericMasterList(List<string> TypeList, List<string> GroupList)
        {
            try
            {
                var genericMasterList = await GetGenericMaster();

                var typeSet = new HashSet<string>(TypeList ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
                var groupSet = new HashSet<string>(GroupList ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);

                var getGenericMaster = genericMasterList
                    .Where(a =>
                        (!typeSet.Any() && !groupSet.Any()) ||
                        ((typeSet.Any() && !string.IsNullOrEmpty(a.Type) && typeSet.Contains(a.Type)) ||
                         (groupSet.Any() && !string.IsNullOrEmpty(a.Group) && groupSet.Contains(a.Group)))
                    )
                    .ToList();

                return getGenericMaster;
            }
            catch (Exception ex)
            {
                _logger.LogError("MasterProjectsDataAccess.GetList - " + ex.Message);
                throw;
            }
        }

        public async Task<List<GenericMasterList>> GetGenericMaster()
        {
            try
            {
                _logger.LogInformation("MasterProjectsDataAccess.GetList - In process");

                //List<Domain.Entities.GenericMasterListSql> genericMasterLists = (List<Domain.Entities.GenericMasterListSql>)await _memoryCacheService.GetCacheValueAsync(_cacheKeyForMaster);

                //if (genericMasterLists != null)
                //{
                //    _logger.LogInformation("Returned from CacheKey: " + _cacheKeyForMaster);
                //    return genericMasterLists;
                //}
                List<Domain.Entities.GenericMasterList> genericMasterLists = new List<GenericMasterList>();

                // Use Dapper's QueryAsync method to execute the query and map results to GenericMasterListSql objects
                var query = @"
                    SELECT 
                    ""Id"", ""ParentId"", ""Group"", ""Type"", ""Sequence"", ""DisplayName"",
                    ""IsDeleted"", ""IsAuthorized"", ""IsActive"",
                    ""CreatedBy"", ""CreatedDateTime"", ""UpdatedBy"", ""UpdatedDateTime"",
                    ""UpdateReason"", ""OwnerId"", ""IsApproved"", ""ApproverId"",
                    ""ApprovedDateTime"", ""AuthorizedById"", ""AuthorizedDateTime"",
                    ""TenantId"", ""SubTenantId"", ""SysData"", ""CustomFields""
                     FROM ""GenericMaster""
                     WHERE ""IsDeleted"" = false
                    ORDER BY
                    CASE WHEN ""Sequence"" > 0 THEN ""Sequence"" END ASC,
                    ""DisplayName"" ASC;";

                using (var connection = new NpgsqlConnection(_configuration["ConnectionStrings:PostgreSqlDBConnection"]))
                {
                    // Dapper automatically handles parameterization
                    var result = await connection.QueryAsync<GenericMasterList>(query).ConfigureAwait(false);

                    _logger.LogInformation("MasterProjectsDataAccess.GetList - Completed");

                    var lst = result.ToList();

                    // Store data in the cache
                    //await _memoryCacheService.SetCacheValueAsync(_cacheKeyForMaster, lst, TimeSpan.FromHours(8));

                    return lst;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("MasterProjectsDataAccess.GetList - " + ex.Message);
                throw;
            }
        }
    }
}
