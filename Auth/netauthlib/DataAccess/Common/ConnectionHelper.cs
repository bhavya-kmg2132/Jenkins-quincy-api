using System.Data;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NetAuth.Application.Common.Interfaces;
using NetAuth.Domain.Enums;
using Npgsql;

namespace NetAuth.DataAccess.Common
{
    internal class ConnectionHelper : IConnectionHelper
    {
        private readonly IConfiguration _configuration;

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
        public Dictionary<string, string> LoadSqlQueriesXml(string xmlFileName)
        {
            //Step 1: Get Db Server name
            SqlDatabaseServer sqlDbServer = GetSqlDbServerFromConfig();
            string xmlFilePath = string.Empty;

            if (sqlDbServer == SqlDatabaseServer.MsSqlServer)
            {
                xmlFilePath = _configuration["MsSqlQueriesXmlPath:" + xmlFileName];
            }
            else if (sqlDbServer == SqlDatabaseServer.PostgreSql)
            {
                xmlFilePath = _configuration["PostgreSqlQueriesXmlPath:" + xmlFileName];
            }
            else
            {
                throw new ApplicationException("No Sql Database found;");
            }

            //Step 2: Get Absolute Path of the SQL File
            string absoluteSqlFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlFilePath));

            //Step 3: Load XML File
            var xml = XElement.Load(absoluteSqlFilePath);

            //Step 4: Convert XML Elements to Dictionary
            return xml.Elements("sql")
                      .ToDictionary(e => e.Attribute("name").Value, e => e.Value.Trim());
        }
    }
}
