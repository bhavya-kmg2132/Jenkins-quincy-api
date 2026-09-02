using Application.IntegrationTests.EndPoints;
using NUnit.Framework;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using System.IO;
using RestSharp;
using System.Reflection;
using Application.IntegrationTests.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Linq;

namespace Application.IntegrationTests
{
    using static Testing;

    public class EndpointTestBase
    {
        //public const string ChrisGreenUserId = "60c7521f-9316-4a5f-a478-cffbb0ea0367";

        public Random rnd { get; set; }
        public EndPointsSettings EndPointsSettings { get; private set; } 
        public string ServerUrl { get; set; }
        public string EndPoint { get; set; }
        public string Auth_Token { get; set; }
        public RestRequest Request { get; set; }
        public RestClient Client { get; set; }
        public UserFromToken UserFromToken { get; set; }

        [SetUp]
        public void BaseSetUp()
        {
            rnd = new Random();
            var file = "endpointssettings.json";
            var settings = new object();
            try
            {
                var buildDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var filePath = buildDir + @"\endpointssettings.json";
                settings = JsonLoader.LoadFromFile<dynamic>(filePath);
            }
            catch
            {
                file = "endpointssettings.json";
                settings = JsonLoader.LoadFromFile<dynamic>(file);
            }

            try
            {
                EndPointsSettings = JsonLoader.Deserialize<EndPointsSettings>(settings);
            }
            catch (Exception ex)
            {
                throw;
            }

            ServerUrl = EndPointsSettings.ApiEndPoint.GetTokenServerUrl;
            if (EndPointsSettings.Test_Environment == "azure_dev_test_slot")
            {
                ServerUrl = EndPointsSettings.ApiServer.azure_dev_test_slot;
            }
            else if (EndPointsSettings.Test_Environment == "azure_dev_slot")
            {
                ServerUrl = EndPointsSettings.ApiServer.azure_dev_slot;
            }
            else if (EndPointsSettings.Test_Environment == "azure_qa_slot")
            {
                ServerUrl = EndPointsSettings.ApiServer.azure_qa_slot;
            }
            else if (EndPointsSettings.Test_Environment == "azure_uat_slot")
            {
                ServerUrl = EndPointsSettings.ApiServer.azure_uat_slot;
            }
            else if (EndPointsSettings.Test_Environment == "azure_staging_slot")
            {
                ServerUrl = EndPointsSettings.ApiServer.azure_staging_slot;
            }
            else if (EndPointsSettings.Test_Environment == "azure_production_slot")
            {
                ServerUrl = EndPointsSettings.ApiServer.azure_production_slot;
            }
            else if (EndPointsSettings.Test_Environment == "kmg_dev_server")
            {
                ServerUrl = EndPointsSettings.ApiServer.kmg_dev_server;
            }
            else if (EndPointsSettings.Test_Environment == "kmg_qa_server")
            {
                ServerUrl = EndPointsSettings.ApiServer.kmg_qa_server;
            }
            else if (EndPointsSettings.Test_Environment == "local")
            {
                ServerUrl = EndPointsSettings.ApiServer.local;
            }

            //Initiate objects for derived classes
            Request = new RestRequest();
            Client = new RestClient();

            //Handle auth
            handle_authentication();
        }

        private void handle_authentication()
        {
            
            Auth_Token = EndPointsSettings.AuthorizationToken; //Token from file
            var TokenRequest = new RestRequest();
            //Get Token from server
            if (Testing.TestRunnerUserName.Contains("Chris.Green"))
            {
                TokenRequest = new RestRequest(EndPointsSettings.ApiEndPoint.GetTokenServerUrl + EndPointsSettings.ApiEndPoint.GetTokenForUserByParamCaffeineLamb, Method.GET);
            }
            else if (Testing.TestRunnerUserName.Contains("John.Smith"))
            {
                TokenRequest = new RestRequest(EndPointsSettings.ApiEndPoint.GetTokenServerUrl + EndPointsSettings.ApiEndPoint.GetTokenForJohnSmith, Method.GET);
            }
            else if (Testing.TestRunnerUserName.Contains("Level1User1"))
            {
                TokenRequest = new RestRequest(EndPointsSettings.ApiEndPoint.GetTokenServerUrl + EndPointsSettings.ApiEndPoint.GetTokenForLevel1User1, Method.GET);
            }
            else if (Testing.TestRunnerUserName.Contains("Level2User1"))
            {
                TokenRequest = new RestRequest(EndPointsSettings.ApiEndPoint.GetTokenServerUrl + EndPointsSettings.ApiEndPoint.GetTokenForLevel2User1, Method.GET);
            }
            else if (Testing.TestRunnerUserName.Contains("Level3User1"))
            {
                TokenRequest = new RestRequest(EndPointsSettings.ApiEndPoint.GetTokenServerUrl + EndPointsSettings.ApiEndPoint.GetTokenForLevel3User1, Method.GET);
            }

            var TokenClient = new RestClient();
            var response = TokenClient.Execute(TokenRequest);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicationException("Error:" + response.ErrorMessage);
            }

            var postJsonResult = ((RestSharp.RestResponseBase)response).Content;
            Auth_Token = "Bearer " + JsonConvert.DeserializeObject<string>(postJsonResult);
            DecodeToken(Auth_Token);
        }

        private UserFromToken DecodeToken(string tokenString)
        {
            // a sample jwt encoded token string which is supposed to be extracted from 'Authorization' HTTP header in your Web Api controller
            //var tokenString = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8wMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJpYXQiOiIxNDI4MDM2NTM5IiwibmJmIjoiMTQyODAzNjUzOSIsImV4cCI6IjE0MjgwNDA0MzkiLCJ2ZXIiOiIxLjAiLCJ0aWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJhbXIiOiJwd2QiLCJvaWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJlbWFpbCI6Impkb2VAbGl2ZS5jb20iLCJwdWlkIjoiSm9obiBEb2UiLCJpZHAiOiJsaXZlLmNvbSIsImFsdHNlY2lkIjoiMTpsaXZlLmNvbTowMDAwMDAwMDAwMDAwMDAwIiwic3ViIjoieHh4eHh4eHh4eHh4eHh4eC15eXl5eSIsImdpdmVuX25hbWUiOiJKb2huIiwiZmFtaWx5X25hbWUiOiJEb2UiLCJuYW1lIjoiSm9obiBEb2UiLCJncm91cHMiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJ1bmlxdWVfbmFtZSI6ImxpdmUuY29tI2pkb2VAbGl2ZS5jb20iLCJhcHBpZCI6IjAwMDAwMDAwLTAwMDAtMDAwMC0wMDAwLTAwMDAwMDAwMDAwMCIsImFwcGlkYWNyIjoiMCIsInNjcCI6InVzZXJfaW1wZXJzb25hdGlvbiIsImFjciI6IjEifQ.K7BCa0NO-A5f9exFiWcIXFMGnLmmt3V2HVP0itMT-GsAxnQROWzJFDIQNFo4QhiW0NCCqJykVELeVBCy_7Dex2-szUPZ69rmmDVJhy_qkmAiHhS1mNZDvJ1sB-whb5wOJ_QPIlByVzubhTcNnuliTVjnTeuOurVJJcn0Vugx9UDkGgky0etHXzmKukWYp4nzA68Wf1xnzlMZBz7PfoPGhjgzQfceOkZJVXIBRMB_7tsyW7gYNbHB_aTiT47cEjkh-UdrZEdp2UaAKugC-es3m076kRHMJqx31x-zDLDBttKinRJVPctiqwb1jMOMV6cUAp2E6aMfEbNk_iqX_OKFJg";
            var jwtEncodedString = tokenString;
            if (tokenString.Contains("Bearer"))
            {
                jwtEncodedString = tokenString.Substring(7); // trim 'Bearer ' from the start since its just a prefix for the token string
            }

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            UserFromToken = new UserFromToken() { Roles =new List<string>()};

            UserFromToken.Oid = token.Claims.FirstOrDefault(c => c.Type == "oid")?.Value;
            UserFromToken.UserId = UserFromToken.Oid;
            UserFromToken.Username = token.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            UserFromToken.Name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            UserFromToken.Scope = token.Claims.FirstOrDefault(c => c.Type == "scp")?.Value;

            foreach (var item in token.Claims.Where(c => c.Type == "roles"))
            {
                UserFromToken.Roles.Add(item.Value);
            }


            return UserFromToken;
        }

        [TearDown]
        public void BaseTearDown() { }
    }
}
