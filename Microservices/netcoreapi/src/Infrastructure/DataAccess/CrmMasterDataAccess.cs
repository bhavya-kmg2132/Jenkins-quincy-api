using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Application.Common.Interfaces;
using AutoMapper;
using Dapper.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Infrastructure.DataAccess
{
    public class CrmMasterDataAccess : ICrmMasterDataAccess
    {
        private readonly ILogger<CrmMasterDataAccess> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDomainEventService _domainEventService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IConnectionHelper _connectionHelper;
        private readonly IMapper _mapper;
        public CrmMasterDataAccess(IConfiguration configuration, ILogger<CrmMasterDataAccess> logger, IDomainEventService domainEventService, ICurrentUserService currentUserService, IDapper dapper, IMapper mapper, IConnectionHelper connectionHelper)
        {
            _logger = logger;
            _configuration = configuration;
            _domainEventService = domainEventService;
            _currentUserService = currentUserService;
            _mapper = mapper;
            this._connectionHelper = connectionHelper;
        }

        public Dictionary<string, string> LoadSqlQueries(string filePath)
        {
            string absoluteSqlFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath));
            var xml = XElement.Load(absoluteSqlFilePath);

            return xml.Elements("sql")
                      .ToDictionary(e => e.Attribute("name").Value, e => e.Value.Trim());
        }

        //code added by gunjan
        public async Task<(string JoinClause, string WhereClause)> BuildWhereClause(JObject filters, string moduleName, string searchText)
        {
            var whereClauses = new List<string>();
            var joinClauses = string.Empty;
            Dictionary<string, string> columnMappings = ColumnMapping.GetColumnList(moduleName);

            if (filters != null)
            {
                joinClauses = BuildJoinClause(moduleName, filters);

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
                string safeSearchText = EscapeSqlLiteral(searchText);

                foreach (var column in columnSearchMappings.Values)
                {
                    searchConditions.Add($"({column} = '{safeSearchText}' OR {column} ILIKE '%{safeSearchText}%')");
                }

                if (searchConditions.Any())
                {
                    whereClauses.Add($"({string.Join(" OR ", searchConditions)})");
                }
            }

            var whereClause = whereClauses.Count > 0 ? "AND " + string.Join(" AND ", whereClauses) : string.Empty;
            return (joinClauses, whereClause);
        }

        private string BuildCondition(string column, JToken filterValue)
        {
            if (filterValue is JObject conditions)
            {
                if (conditions.ContainsKey("conditions"))
                {
                    var operatorType = conditions["operator"]?.ToString() ?? "AND";
                    var subConditions = conditions["conditions"];
                    var conditionStrings = new List<string>();

                    foreach (var condition in subConditions)
                    {
                        var filterType = condition.Value<string>("filterType");
                        var conditionString = BuildConditionByFilterType(column, filterType, condition);
                        if (!string.IsNullOrEmpty(conditionString))
                        {
                            conditionStrings.Add(conditionString);
                        }
                    }

                    return $"({string.Join($" {operatorType} ", conditionStrings)})";
                }
                else
                {
                    var filterType = conditions.Value<string>("filterType");
                    return BuildConditionByFilterType(column, filterType, conditions);
                }
            }
            else
            {
                string safeValue = EscapeSqlLiteral(filterValue.ToString());
                return $"{column} ILIKE '%{safeValue}%'";
            }
        }

        private string BuildConditionByFilterType(string column, string filterType, JToken filterValue)
        {
            if (string.IsNullOrEmpty(filterType))
                return string.Empty;

            return filterType switch
            {
                "number" => BuildNumberCondition(column, filterValue),
                "text" => BuildTextCondition(column, filterValue),
                "date" => BuildDateCondition(column, filterValue),
                "set" => BuildSetCondition(column, filterValue),
                _ => $"{column} ILIKE '%{EscapeSqlLiteral(filterValue.ToString())}%'"
            };
        }

        private string BuildNumberCondition(string column, JToken filterValue)
        {
            var conditionType = filterValue.Value<string>("type");
            var filter = filterValue.Value<string>("filter");
            var filterTo = filterValue.Value<string>("filterTo");

            return conditionType switch
            {
                "greaterThan" => $"{column} > {filter}",
                "greaterThanOrEqual" => $"{column} >= {filter}",
                "lessThan" => $"{column} < {filter}",
                "lessThanOrEqual" => $"{column} <= {filter}",
                "equals" => $"{column} = {filter}",
                "notEqual" => $"{column} <> {filter}",
                "inRange" => $"{column} BETWEEN {filter} AND {filterTo}",
                "blank" => $"{column} IS NULL",
                "notBlank" => $"{column} IS NOT NULL",
                _ => throw new ArgumentException($"Unsupported condition type: {conditionType}")
            };
        }

        private string BuildTextCondition(string column, JToken filterValue)
        {
            var conditionType = filterValue.Value<string>("type");
            var rawFilter = filterValue.Value<string>("filter");
            var filterToken = filterValue["filter"];
            string safeFilter = EscapeSqlLiteral(rawFilter);

            if (filterToken == null || rawFilter == null)
            {
                return conditionType switch
                {
                    "contains" or "equals" => $"{column} IS NULL",
                    "notContains" or "notEqual" => $"{column} IS NOT NULL",
                    _ => string.Empty
                };
            }

            return conditionType switch
            {
                "contains" => $"{column} ILIKE '%{safeFilter}%'",
                "notContains" => $"{column} NOT ILIKE '%{safeFilter}%'",
                "equals" => $"{column} = '{safeFilter}'",
                "notEqual" => $"{column} <> '{safeFilter}'",
                "startsWith" => $"{column} ILIKE '{safeFilter}%'",
                "endsWith" => $"{column} ILIKE '%{safeFilter}'",
                "blank" => $"{column} IS NULL",
                "notBlank" => $"{column} IS NOT NULL",
                _ => throw new ArgumentException($"Unsupported text condition type: {conditionType}")
            };
        }

        private string BuildDateCondition(string column, JToken filterValue)
        {
            var conditionType = filterValue.Value<string>("type");
            var filterFrom = EscapeSqlLiteral(filterValue.Value<string>("dateFrom"));
            var filterTo = EscapeSqlLiteral(filterValue.Value<string>("dateTo"));

            return conditionType switch
            {
                "greaterThan" => $"{column} > '{filterFrom}'",
                "lessThan" => $"{column} < '{filterFrom}'",
                "equals" => $"{column} = '{filterFrom}'",
                "notEqual" => $"{column} <> '{filterFrom}'",
                "inRange" => $"{column} BETWEEN '{filterFrom}' AND '{filterTo}'",
                "blank" => $"{column} IS NULL",
                "notBlank" => $"{column} IS NOT NULL",
                _ => throw new ArgumentException($"Unsupported date condition type: {conditionType}")
            };
        }

        private string BuildSetCondition(string column, JToken filterValue)
        {
            var valuesToken = filterValue["values"];
            List<string> values;

            if (valuesToken is JArray array)
            {
                values = array.Select(v => $"'{EscapeSqlLiteral(v.ToString().Trim())}'").ToList();
            }
            else
            {
                var raw = EscapeSqlLiteral(filterValue.Value<string>("values") ?? "");
                values = raw.Split(',').Select(v => $"'{v.Trim()}'").ToList();
            }

            return $"{column} IN ({string.Join(",", values)})";
        }

        private string BuildJoinClause(string moduleName, JObject filters)
        {
            var joinClauses = new List<string>();

            if (!ColumnMapping.JoinRelations.ContainsKey(moduleName))
                return string.Empty;

            foreach (var (relatedTable, condition, columns) in ColumnMapping.JoinRelations[moduleName])
            {
                bool shouldJoin = filters.Properties().Any(p => columns.Contains(p.Name));

                if (shouldJoin)
                {
                    joinClauses.Add($"LEFT JOIN {relatedTable} ON {condition}");
                }
            }

            return joinClauses.Any() ? string.Join(" ", joinClauses) : string.Empty;
        }

        private string EscapeSqlLiteral(string input) =>
            input?.Replace("'", "''");

        public static class ColumnMapping
        {

            public static readonly Dictionary<string, string> PublishEventDataColumnList = new Dictionary<string, string>()
{
         { "id", "\"Es\".\"Id\"" },
         { "correlationId", "\"Es\".\"CorrelationId\"" },
         { "auditableRequestId", "\"Es\".\"AuditableRequestId\"" },
         { "auditableRequestName", "\"Es\".\"AuditableRequestName\"" },
         { "auditableAssemblyQualifiedName", "\"Es\".\"AuditableAssemblyQualifiedName\"" },
         { "auditableSourceEventName", "\"Es\".\"AuditableSourceEventName\"" },
         { "createdDateTime", "\"Es\".\"CreatedDateTime\"" },
         { "apiName", "\"Es\".\"ApiName\"" },
         { "collectionName", "\"Es\".\"CollectionName\"" },
         { "eventData", "\"Es\".\"EventData\"" },
         { "userId", "\"Es\".\"UserId\"" },
         { "operationType", "\"Es\".\"OperationType\"" }

    };

            public static readonly Dictionary<string, string> PublishEventDataSearchColumnList = new Dictionary<string, string>()
    {

        { "auditableRequestName", "\"Es\".\"AuditableRequestName\"" },

        { "auditableAssemblyQualifiedName", "\"Es\".\"AuditableAssemblyQualifiedName\"" },

        { "auditableSourceEventName", "\"Es\".\"AuditableSourceEventName\"" },

        { "operationType", "\"Es\".\"OperationType\"" }

    };

            public static readonly Dictionary<string, List<(string Table, string Condition, List<string> Columns)>> JoinRelations =
            new Dictionary<string, List<(string Table, string Condition, List<string> Columns)>>()
     {
        { "Lead", new List<(string, string, List<string>)>
            {
                (
                    "\"LeadCampaign\"",
                    "\"L\".\"Id\" = \"LeadCampaign\".\"LeadId\" AND \"LeadCampaign\".\"IsActive\" = true",
                    new List<string> { "\"campaignId\"" }
                )
            }
        },
        { "Campaign", new List<(string, string, List<string>)>
            {
                (
                    "\"LeadCampaign\"",
                    "\"L\".\"Id\" = \"LeadCampaign\".\"CampaignId\" AND \"LeadCampaign\".\"IsActive\" = true",
                    new List<string> { "\"leadId\"" }
                ),
                (
                    "\"ContactCampaign\"",
                    "\"C\".\"Id\" = \"ContactCampaign\".\"CampaignId\"",
                    new List<string> { "\"contactId\"" }
                )
            }
        },
        { "Contact", new List<(string, string, List<string>)>
            {
                (
                    "\"ContactCampaign\"",
                    "\"C\".\"Id\" = \"ContactCampaign\".\"ContactId\" AND \"ContactCampaign\".\"IsActive\" = true",
                    new List<string> { "\"campaignId\"" }
                )
            }
        }
     };

            public static Dictionary<string, string> GetColumnList(string moduleName, bool isSearchColumn = false)
            {
                return (moduleName, isSearchColumn) switch
                {

                    ("PublishEventData", false) => PublishEventDataColumnList,
                    ("PublishEventData", true) => PublishEventDataSearchColumnList,

                    _ => throw new ArgumentException($"Unknown module: {moduleName}")
                };
            }
        }


    }
}