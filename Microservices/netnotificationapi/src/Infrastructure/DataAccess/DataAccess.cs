using System;
using System.IO;
using System.Reflection;
using Infrastructure.DataAccess.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.DataAccess
{
    public class DataAccess
    {
        protected ILogger<DataAccess> logger { get; private set; }
        protected IConfiguration configuration { get; private set; }

        protected readonly DataSettings dataSettings;
        public bool IsAccessLookUpData { get; set; }


        /// <summary>
        /// Data access
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public DataAccess(IConfiguration configuration, ILogger<DataAccess> logger)
        {
            this.configuration = configuration;
            this.logger = logger;

            //config["ConnectionStrings:SqlDBConnection"]
            //var issuer = Configuration["JwtBearerSettings:Issuer"];

            if (this.configuration != null)
            {
                //Load from configuration
                dataSettings = new DataSettings();
                dataSettings.SqlServerConnectionString = this.configuration["ConnectionStrings:SqlDBConnection"];
                dataSettings.NoSqlDBConnectionString = this.configuration["ConnectionStrings:NoSqlDBConnection"];
                IsAccessLookUpData = true;

                if (!string.IsNullOrEmpty(dataSettings.SqlServerConnectionString))
                {
                    SqlServerConnection(logger);
                }
            }
        }

        private bool SqlServerConnection(ILogger<DataAccess> logger)
        {
            SqlConnectionStringBuilder sqlconnectionbuilder = new SqlConnectionStringBuilder(this.configuration["ConnectionStrings:SqlDBConnection"]);
            var dbName = sqlconnectionbuilder.InitialCatalog;
            var dbServer = sqlconnectionbuilder.DataSource;

            logger.LogInformation("SqlConnection: " + this.dataSettings.SqlServerConnectionString);
            logger.LogInformation("Database Name: " + dbName);
            logger.LogInformation("Database Server: " + dbServer);

            return true;
        }

        public bool MongoDBConnection(ILogger<DataAccess> logger)
        {
            if (string.IsNullOrEmpty(dataSettings.NoSqlDBConnectionString))
            {
                return false;
            }

            // Replace the placeholder with your Atlas connection string
            string connectionUri = dataSettings.NoSqlDBConnectionString;

            var settings = MongoClientSettings.FromConnectionString(connectionUri);

            // Set the ServerApi field of the settings object to Stable API version 1
            //settings.ServerApi = new MongoDB.Driver.ServerApi(MongoDB.Driver.ServerApiVersion.V1);

            // Create a new client and connect to the server
            var client = new MongoClient(settings);

            // Send a ping to confirm a successful connection
            try
            {
                var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                logger.LogInformation("Pinged your deployment. You successfully connected to MongoDB!");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return false;
            }
        }

        public bool TestSQLServerDBConnection()
        {
            SqlConnection sqlConnection = new SqlConnection(dataSettings.SqlServerConnectionString);
            try
            {
                sqlConnection.Open();
                return true;
            }
            catch (Exception)
            {
                sqlConnection.Close();
                return false;
            }
        }

        public bool SqlConnectionFromLocalDataSettingsFile()
        {

            var file = "datasettings.json";
            var settings = new object();

            try
            {
                var buildDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var filePath = buildDir + @"\datasettings.json";
                settings = JsonLoader.LoadFromFile<dynamic>(filePath);
                IsAccessLookUpData = true;
            }
            catch
            {
                file = "datasettings.json";
                settings = JsonLoader.LoadFromFile<dynamic>(file);
            }

            try
            {
                var dataSettings1 = JsonLoader.Deserialize<DataSettings>(settings);
                IsAccessLookUpData = true;
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError("DataAccess - Could not find datasettings.json");
                logger.LogError("DataAccess - " + ex.Message);
                return false;
            }
        }
    }
}
