using System;
using System.IO;
using Application.Common.Interfaces;
using DbUp;
using DbUp.Engine;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.BuildScripts
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly string _sqlConnectionString;
        private readonly string _netAuthSqlServerConnectionString;
        private readonly string _netAuthPostgreSqlConnectionString;
        private readonly bool _runNetAuthInitialSetUpScripts;
        private readonly bool _runEventDbInitialSetUpScripts;

        private readonly IConfiguration _configuration;
        private readonly string _postgreSqlConnectionString;
        private readonly string _eventDbConnectionString;
        private readonly ILogger<DatabaseInitializer> _logger;


        public DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _sqlConnectionString = configuration.GetConnectionString("SqlDBConnection");
            _runNetAuthInitialSetUpScripts = configuration.GetSection("Api:RunNetAuthInitialSetUpScripts").Get<bool>();
            _runEventDbInitialSetUpScripts = configuration.GetSection("Api:RunEventDbInitialSetUpScripts").Get<bool>();
            _netAuthSqlServerConnectionString = configuration["NetAuth.ConnectionStrings:SqlDBConnection"];
            _netAuthPostgreSqlConnectionString = configuration["NetAuth.ConnectionStrings:PostgreSqlDBConnection"];
            _postgreSqlConnectionString = configuration.GetConnectionString("PostgreSqlDBConnection");
            _eventDbConnectionString = configuration.GetConnectionString("AzureEventDBPostgreSqlDBConnection");
        }

        public async void Initialize()
        {

            try
            {
                _logger.LogInformation($"DatabaseInitializer - Process Started...");

                if (_configuration["Api:SqlDatabaseServer"] == SqlDatabaseServer.MsSqlServer.ToString())
                {
                    InitializeMsSqlServerDatabase();

                    if (_runNetAuthInitialSetUpScripts)
                    {
                        InitializeNetAuthMsSqlServerDatabase();
                    }
                }

                else if (_configuration["Api:SqlDatabaseServer"] == SqlDatabaseServer.PostgreSql.ToString())
                {
                    InitializePostgreSqlDatabase();

                    if (_runNetAuthInitialSetUpScripts)
                    {
                        InitializeNetAuthPostgreSqlDatabase();
                    }
                }

                if (_runEventDbInitialSetUpScripts)
                {
                    InitializeEventDbDatabase();
                }

                _logger.LogInformation($"DatabaseInitializer - Process Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DatabaseInitializer - An error occurred during initialization : {ex.Message}");
                throw;
            }


        }

        private void InitializeMsSqlServerDatabase()
        {
            if (_configuration["Api:SqlDatabaseServer"] == SqlDatabaseServer.MsSqlServer.ToString())
            {
                _logger.LogInformation("Initializing MsSQL Server database...");

                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string relativePath = @"Persistence\BuildScripts\Scripts\MsSqlServer";
                string fullPath = Path.GetFullPath(Path.Combine(currentDirectory, relativePath));

                // Ensure the database exists
                EnsureDatabase.For.SqlDatabase(_sqlConnectionString);

                var upgrader = DeployChanges.To
                                           .SqlDatabase(_sqlConnectionString)
                                           .WithScriptsFromFileSystem(fullPath)
                                           .LogToConsole()
                                           .Build();

                PerformUpgrade(upgrader, "MsSqlServer");
            }
        }

        private void InitializePostgreSqlDatabase()
        {
            if (_configuration["Api:SqlDatabaseServer"] == SqlDatabaseServer.PostgreSql.ToString())
            {
                _logger.LogInformation("Initializing PostgreSQL database...");

                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string relativePath = @"Persistence\BuildScripts\Scripts\PostgreSql";
                string fullPath = Path.GetFullPath(Path.Combine(currentDirectory, relativePath));

                // Ensure the database exists (user in connection string needs create database permissions)
                EnsureDatabase.For.PostgresqlDatabase(_postgreSqlConnectionString);

                var upgrader = DeployChanges.To
                                           .PostgresqlDatabase(_postgreSqlConnectionString)
                                           .WithScriptsFromFileSystem(fullPath)
                                           .LogToConsole()
                                           .Build();

                PerformUpgrade(upgrader, "PostgreSql");
            }
        }

        private void InitializeNetAuthMsSqlServerDatabase()
        {
            if (_configuration["Api:SqlDatabaseServer"] == SqlDatabaseServer.MsSqlServer.ToString())
            {
                _logger.LogInformation("Initializing NetAuth MsSQL Server database...");

                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string relativePath = @"Persistence\BuildScripts\NetAuth\Scripts\MsSqlServer";
                string fullPath = Path.GetFullPath(Path.Combine(currentDirectory, relativePath));

                // Ensure the database exists
                EnsureDatabase.For.SqlDatabase(_netAuthSqlServerConnectionString);

                var upgrader = DeployChanges.To
                                           .SqlDatabase(_netAuthSqlServerConnectionString)
                                           .WithScriptsFromFileSystem(fullPath)
                                           .LogToConsole()
                                           .Build();

                RunNetAuthInitialSetUpScripts(upgrader, "MsSqlServer");
            }
        }

        private void InitializeNetAuthPostgreSqlDatabase()
        {
            if (_configuration["Api:SqlDatabaseServer"] == SqlDatabaseServer.PostgreSql.ToString())
            {
                _logger.LogInformation("Initializing NetAuth PostgreSQL database...");

                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string relativePath = @"Persistence\BuildScripts\NetAuth\Scripts\PostgreSql";
                string fullPath = Path.GetFullPath(Path.Combine(currentDirectory, relativePath));

                // Ensure the database exists (user in connection string needs create database permissions)
                EnsureDatabase.For.PostgresqlDatabase(_netAuthPostgreSqlConnectionString);

                var upgrader = DeployChanges.To
                                           .PostgresqlDatabase(_netAuthPostgreSqlConnectionString)
                                           .WithScriptsFromFileSystem(fullPath)
                                           .LogToConsole()
                                           .Build();

                RunNetAuthInitialSetUpScripts(upgrader, "PostgreSql");
            }
        }

        private void InitializeEventDbDatabase()
        {
            _logger.LogInformation("Initializing EventDB PostgreSQL database...");

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = @"Persistence\BuildScripts\EventDb\Scripts\PostgreSql";
            string fullPath = Path.GetFullPath(Path.Combine(currentDirectory, relativePath));

            EnsureDatabase.For.PostgresqlDatabase(_eventDbConnectionString);

            var upgrader = DeployChanges.To
                                       .PostgresqlDatabase(_eventDbConnectionString)
                                       .WithScriptsFromFileSystem(fullPath)
                                       .LogToConsole()
                                       .Build();

            PerformUpgrade(upgrader, "EventDB PostgreSql");
        }

        private void PerformUpgrade(UpgradeEngine upgrader, string dbTypeName)
        {
            _logger.LogInformation($"Checking for {dbTypeName} database upgrades...");

            if (upgrader.IsUpgradeRequired())
            {
                _logger.LogInformation($"Upgrade is required for {dbTypeName}. Performing upgrade...");
                var result = upgrader.PerformUpgrade();

                if (result.Successful)
                {
                    _logger.LogInformation($"{dbTypeName} database upgrade completed successfully.");
                }
                else
                {
                    _logger.LogError($"Database upgrade failed for {dbTypeName}: {result.Error.Message}");
                }
            }
            else
            {
                _logger.LogInformation($"No {dbTypeName} database upgrade required.");
            }
        }
        private void RunNetAuthInitialSetUpScripts(UpgradeEngine upgrader, string dbTypeName)
        {
            _logger.LogInformation($"Checking for {dbTypeName} database upgrades...");

            if (upgrader.IsUpgradeRequired())
            {
                _logger.LogInformation($"Upgrade is required for {dbTypeName}. Performing upgrade...");
                var result = upgrader.PerformUpgrade();

                if (result.Successful)
                {
                    _logger.LogInformation($"{dbTypeName} NetAuth Initial SetUp Script are executed successfully.");
                }
                else
                {
                    _logger.LogError($"NetAuth Initial SetUp Script are failed for {dbTypeName}: {result.Error.Message}");
                }
            }
            else
            {
                _logger.LogInformation($"No NetAuth {dbTypeName} database upgrade required.");
            }
        }
    }
}