using System;
using System.IO;
using System.Reflection;
using Infrastructure.DataAccess.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
