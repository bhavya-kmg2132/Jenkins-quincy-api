//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Microsoft.Data.SqlClient;
//using System.Reflection;

//namespace netauthlib
//{
//    public class NetAuthBase
//    {
//        //private readonly IConfiguration _configuration;
//        //private readonly ILogger<AuthController> _logger;
//        //private string _clientId = string.Empty;
//        //private string _clientSecretValue = string.Empty;
//        //private string _tenantId = string.Empty;
//        //private string _exposedApiScope = string.Empty;
//        //private string _username = "Dave@stai09.onmicrosoft.com";
//        //private string _password = "Diagnostics11";

//        //private string uri_token = "https://login.microsoftonline.com/9c822166-54c6-4c9c-b0eb-d390a8f54e66/oauth2/v2.0/token";
//        //private string uri_authorize = "https://login.microsoftonline.com/9c822166-54c6-4c9c-b0eb-d390a8f54e66/oauth2/v2.0/authorize?";

//        Public static readonly string[] Summaries = new[]
//        {
//            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//        };

//        // The Web API will only accept tokens 1) for users, and 2) having the "access_as_user" scope for this API

//    }

//    public IEnumerable<string> HeartBeat()
//        {
//            SqlConnectionStringBuilder sqlconnectionbuilder = new SqlConnectionStringBuilder(this._Configuration["NetAuth.ConnectionStrings:SqlDBConnection"]);
//            var dbName = sqlconnectionbuilder.InitialCatalog;
//            var dbServer = sqlconnectionbuilder.DataSource;
//            var dbUser = sqlconnectionbuilder.UserID;
//            string timeStampId = Convert.ToString(Guid.NewGuid());

//            _Logger.LogInformation("TimeStampId: " + timeStampId);
//            _Logger.LogInformation("Api Internal Name: " + this._Configuration["NetAuth.Api:internal_name"]);
//            _Logger.LogInformation("Api Code: " + this._Configuration["NetAuth.Api:code"]);
//            _Logger.LogInformation("Environment: " + this._environment.EnvironmentName);
//            _Logger.LogInformation("Product: " + Convert.ToString(Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product));
//            _Logger.LogInformation("Company: " + Convert.ToString(Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company));
//            _Logger.LogInformation("Product/Package Version (InformationalVersion): " + Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
//            _Logger.LogInformation("File Version: " + Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
//            _Logger.LogInformation("Environment: " + this._environment.EnvironmentName);
//            _Logger.LogInformation("Database Server: " + dbServer);
//            _Logger.LogInformation("Database Name: " + dbName);
//            _Logger.LogInformation("Database User: " + dbUser);

//            return new string[] {
//                "TimeStampId: " + timeStampId,
//                "Api Internal Name: "+ this._Configuration["NetAuth.Api:internal_name"],
//                "Api Code: "+ this._Configuration["NetAuth.Api:code"],
//                "Product: " + Convert.ToString(Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product),
//                //"Description: " + Convert.ToString(Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>().Description),
//                "Company: " + Convert.ToString(Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company),
//                //"Copyright: " + Convert.ToString(Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright),
//                "Product/Package Version (InformationalVersion): " + Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion,
//                "File Version: " + Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version,
//                "Environment: " + this._environment.EnvironmentName,
//                "Database Server: " + dbServer,
//                "Database Name: "+ dbName,
//                "Database User: "+ dbUser
//            };
//        }
//    }
