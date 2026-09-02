using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Application.Common.Interfaces;
using Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DataAccess.Common
{
    public class ConnectionHelper : IConnectionHelper
    {
        private readonly IConfiguration _configuration;

        // Maps each DbConfigKeys enum value to the appsettings key that holds the DB server type.
        private static readonly Dictionary<DbConfigKeys, string> _dbTypeConfigKeyMap = new()
        {
            [DbConfigKeys.MainDb] = "Api:SqlDatabaseServer",
            [DbConfigKeys.EventDb] = "Api:EventDatabase",
            [DbConfigKeys.NetAuthDb] = "Api:SqlDatabaseServer"
        };

        // Keyed by "DbConfigKeys.Name:xmlFileName" — populated once per unique pair, reused forever.
        private static readonly ConcurrentDictionary<string, Dictionary<string, string>> _queryCache = new();

        public ConnectionHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            var dbType = ParseDbType(_configuration["Api:SqlDatabaseServer"], "Api:SqlDatabaseServer");
            var connectionString = GetConnectionString(dbType);
            return Create(dbType, connectionString);
        }
        public IDbConnection CreateNetAuthConnection()
        {
            var dbType = ParseDbType(_configuration["Api:SqlDatabaseServer"], "Api:SqlDatabaseServer");

            string key = dbType == SqlDatabaseServer.MsSqlServer
                ? "NetAuth.ConnectionStrings:SqlDBConnection"
                : "NetAuth.ConnectionStrings:PostgreSqlDBConnection";

            var connectionString = _configuration[key];

            return Create(dbType, connectionString);
        }

        public IDbConnection CreateEventConnection()
        {
            var dbType = ParseDbType(_configuration["Api:EventDatabase"], "Api:EventDatabase");

            string key = "ConnectionStrings:AzureEventDBPostgreSqlDBConnection";

            var connectionString = _configuration[key];

            return Create(dbType, connectionString);
        }

        private SqlDatabaseServer GetSqlDbServerFromConfig()
        {
            return ParseDbType(_configuration["Api:SqlDatabaseServer"], "Api:SqlDatabaseServer"); ;
        }

        private SqlDatabaseServer ParseDbType(string? value, string configKey)
        {
            if (!Enum.TryParse(value, true, out SqlDatabaseServer dbType))
                throw new ArgumentException($"Invalid or missing database type in {configKey}");

            return dbType;
        }

        private string GetConnectionString(SqlDatabaseServer dbType)
        {
            string key = dbType == SqlDatabaseServer.MsSqlServer
                ? "ConnectionStrings:SqlDBConnection"
                : "ConnectionStrings:PostgreSqlDBConnection";

            return _configuration[key];
        }


        private IDbConnection Create(SqlDatabaseServer type, string connStr)
        {
            IDbConnection conn = type switch
            {
                SqlDatabaseServer.MsSqlServer => new SqlConnection(connStr),
                SqlDatabaseServer.PostgreSql => new NpgsqlConnection(connStr),
                _ => throw new ArgumentException($"Unsupported DB: {type}")
            };

            return conn;
        }

        /// <summary>
        /// This method loads SQL queries from an XML file and stores them in a Dictionary<string, string>, where:
        /// The key is the query name(from the XML attribute name).
        /// The value is the actual SQL query(from the XML content).
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public Dictionary<string, string> LoadSqlQueriesXml(string xmlFileName, DbConfigKeys dbKey = DbConfigKeys.MainDb)
        {
            var cacheKey = $"{dbKey}:{xmlFileName}";

            return _queryCache.GetOrAdd(cacheKey, _ =>
            {
                var configKey = _dbTypeConfigKeyMap[dbKey];
                var dbType = ParseDbType(_configuration[configKey], configKey);

                string xmlFilePath = dbType switch
                {
                    SqlDatabaseServer.MsSqlServer => _configuration["MsSqlQueriesXmlPath:" + xmlFileName],
                    SqlDatabaseServer.PostgreSql => _configuration["PostgreSqlQueriesXmlPath:" + xmlFileName],
                    _ => throw new ApplicationException($"Unsupported database type '{dbType}' for key '{configKey}'")
                };

                string absoluteSqlFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlFilePath));
                var xml = XElement.Load(absoluteSqlFilePath);
                return xml.Elements("sql")
                          .ToDictionary(e => e.Attribute("name").Value, e => e.Value.Trim());
            });
        }
    }
}
