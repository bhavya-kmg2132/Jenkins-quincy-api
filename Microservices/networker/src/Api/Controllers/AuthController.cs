using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private string _clientId = string.Empty;
        private string _clientSecretValue = string.Empty;
        private string _tenantId = string.Empty;
        private string _exposedApiScope = string.Empty;
        private string _username = "Dave@stai09.onmicrosoft.com";
        private string _password = "Diagnostics11";

        private string uri_token = "https://login.microsoftonline.com/9c822166-54c6-4c9c-b0eb-d390a8f54e66/oauth2/v2.0/token";
        private string uri_authorize = "https://login.microsoftonline.com/9c822166-54c6-4c9c-b0eb-d390a8f54e66/oauth2/v2.0/authorize?";

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        // The Web API will only accept tokens 1) for users, and 2) having the "access_as_user" scope for this API

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            this._tenantId = this._configuration["AzureAd:TenantId"];
            this._clientId = this._configuration["AzureAd:ClientId"];
            this._exposedApiScope = this._configuration["AzureAd:ExposedApiScope"];
            this._clientSecretValue = this._configuration["AzureAd:SecretValue"];
            this.uri_token = $"https://login.microsoftonline.com/{_tenantId}/oauth2/v2.0/token";
            this.uri_authorize = $"https://login.microsoftonline.com/{_tenantId}/oauth2/v2.0/authorize?";
        }



        /// <summary>
        /// Get token for user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetTokenForUserByParam")]
        public async Task<string> GetTokenForUserByParam(string username, string password)
        {

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("resource", _clientId),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecretValue),
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("redirect_uri", "http://localhost/myapp/"),
                //new KeyValuePair<string, string>("scope", "api://52a29dcb-f11c-4851-b8ac-3e680a4ab403/access_as_user")
             };
            const string TokenEndpoint = "https://login.microsoftonline.com/{0}/oauth2/v2.0/token";
            const string requestParam = "scope={0}&client_id={1}&grant_type=password&client_secret={2}&username={3}&password={4}";

            var payload = String.Format(requestParam,
                                WebUtility.UrlEncode(_exposedApiScope),
                                WebUtility.UrlEncode(_clientId),
                                WebUtility.UrlEncode(_clientSecretValue),
                                WebUtility.UrlEncode(username),
                                WebUtility.UrlEncode(password));

            string token = string.Empty;
            using (var client = new HttpClient())
            {
                var address = String.Format(TokenEndpoint, _tenantId);
                var content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");
                using (var response = await client.PostAsync(address, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var result_content = await response.Content.ReadAsStringAsync();
                        return result_content;
                        //Console.WriteLine("Status:  {0}", response.StatusCode);
                        //Console.WriteLine("Content: {0}", await response.Content.ReadAsStringAsync());
                    }

                    response.EnsureSuccessStatusCode();
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
                    //JObject json = JObject.Parse(responseContent);

                    token = tokenResponse.access_token;
                }
            }

            //if (token != string.Empty)
            //{
            //    // Do API Calls
            //    using (var client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            //        client.BaseAddress = new Uri("https://management.azure.com/");


            //        //await ListAllResourcesInSubscriptionWithPaging(client);
            //        //await DeleteEmptyResourceGroups(client);
            //        //await TestWebAppOperations(client);
            //        //await TestFunctionAppOperations(client);
            //    }
            //}
            //if (!string.IsNullOrEmpty(token))
            //{
            //    CreateUserRequest request = new CreateUserRequest();
            //    request.UserName = username;
            //    await Mediator.Send(request);
            //}
            return token;
        }


        /// <summary>
        /// GetTokenForChrisGreen
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetTokenForChrisGreen")]
        public async Task<string> GetTokenForChrisGreen()
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("resource", _clientId),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecretValue),
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", "Chris.Green@stai09.onmicrosoft.com"),
                new KeyValuePair<string, string>("password", "Diagnostics11"),
                new KeyValuePair<string, string>("redirect_uri", "http://localhost/myapp/"),
                //new KeyValuePair<string, string>("scope", $"api://{_exposedApiScope}")
             };
            const string TokenEndpoint = "https://login.microsoftonline.com/{0}/oauth2/v2.0/token";
            const string requestParam = "scope={0}&client_id={1}&grant_type=password&client_secret={2}&username={3}&password={4}";

            var payload = String.Format(requestParam,
                                WebUtility.UrlEncode(_exposedApiScope),
                                WebUtility.UrlEncode(_clientId),
                                WebUtility.UrlEncode(_clientSecretValue),
                                WebUtility.UrlEncode("Chris.Green@stai09.onmicrosoft.com"),
                                WebUtility.UrlEncode("Diagnostics11"));

            string token = string.Empty;
            using (var client = new HttpClient())
            {
                var address = String.Format(TokenEndpoint, _tenantId);
                var content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");
                using (var response = await client.PostAsync(address, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var result_content = await response.Content.ReadAsStringAsync();
                        return result_content;
                        //Console.WriteLine("Status:  {0}", response.StatusCode);
                        //Console.WriteLine("Content: {0}", await response.Content.ReadAsStringAsync());
                    }

                    response.EnsureSuccessStatusCode();
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
                    //JObject json = JObject.Parse(responseContent);

                    token = tokenResponse.access_token;
                }
            }

            //if (token != string.Empty)
            //{
            //    // Do API Calls
            //    using (var client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            //        client.BaseAddress = new Uri("https://management.azure.com/");


            //        //await ListAllResourcesInSubscriptionWithPaging(client);
            //        //await DeleteEmptyResourceGroups(client);
            //        //await TestWebAppOperations(client);
            //        //await TestFunctionAppOperations(client);
            //    }
            //}
            //if (!string.IsNullOrEmpty(token))
            //{
            //    CreateUserRequest request = new CreateUserRequest();
            //    request.UserName = _username;
            //    await Mediator.Send(request);
            //}
            return token;
        }

        [AllowAnonymous]
        [HttpGet("GetTokenForDaemon")]
        public async Task<string> GetTokenForDaemon()
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("resource", _clientId),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecretValue),
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", _username),
                new KeyValuePair<string, string>("password", _password),
                new KeyValuePair<string, string>("redirect_uri", "http://localhost/myapp/"),
                //new KeyValuePair<string, string>("scope", "api://52a29dcb-f11c-4851-b8ac-3e680a4ab403/.default")
             };
            const string TokenEndpoint = "https://login.microsoftonline.com/{0}/oauth2/v2.0/token";
            const string requestParam = "scope=api://52a29dcb-f11c-4851-b8ac-3e680a4ab403/.default&client_id={0}&grant_type=client_credentials&client_secret={1}";

            var payload = String.Format(requestParam,
                                WebUtility.UrlEncode(_clientId),
                                WebUtility.UrlEncode(_clientSecretValue));

            string token = string.Empty;
            using (var client = new HttpClient())
            {
                var address = String.Format(TokenEndpoint, _tenantId);
                var content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");
                using (var response = await client.PostAsync(address, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var result_content = await response.Content.ReadAsStringAsync();
                        return result_content;
                        //Console.WriteLine("Status:  {0}", response.StatusCode);
                        //Console.WriteLine("Content: {0}", await response.Content.ReadAsStringAsync());
                    }

                    response.EnsureSuccessStatusCode();
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
                    //JObject json = JObject.Parse(responseContent);

                    token = tokenResponse.access_token;
                }
            }

            //if (token != string.Empty)
            //{
            //    // Do API Calls
            //    using (var client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            //        client.BaseAddress = new Uri("https://management.azure.com/");


            //        //await ListAllResourcesInSubscriptionWithPaging(client);
            //        //await DeleteEmptyResourceGroups(client);
            //        //await TestWebAppOperations(client);
            //        //await TestFunctionAppOperations(client);
            //    }
            //}

            return token;
        }

        [AllowAnonymous]
        [HttpGet("DecodeToken")]
        public JsonResult DecodeToken(string tokenString)
        {
            // a sample jwt encoded token string which is supposed to be extracted from 'Authorization' HTTP header in your Web Api controller
            //var tokenString = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8wMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJpYXQiOiIxNDI4MDM2NTM5IiwibmJmIjoiMTQyODAzNjUzOSIsImV4cCI6IjE0MjgwNDA0MzkiLCJ2ZXIiOiIxLjAiLCJ0aWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJhbXIiOiJwd2QiLCJvaWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJlbWFpbCI6Impkb2VAbGl2ZS5jb20iLCJwdWlkIjoiSm9obiBEb2UiLCJpZHAiOiJsaXZlLmNvbSIsImFsdHNlY2lkIjoiMTpsaXZlLmNvbTowMDAwMDAwMDAwMDAwMDAwIiwic3ViIjoieHh4eHh4eHh4eHh4eHh4eC15eXl5eSIsImdpdmVuX25hbWUiOiJKb2huIiwiZmFtaWx5X25hbWUiOiJEb2UiLCJuYW1lIjoiSm9obiBEb2UiLCJncm91cHMiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJ1bmlxdWVfbmFtZSI6ImxpdmUuY29tI2pkb2VAbGl2ZS5jb20iLCJhcHBpZCI6IjAwMDAwMDAwLTAwMDAtMDAwMC0wMDAwLTAwMDAwMDAwMDAwMCIsImFwcGlkYWNyIjoiMCIsInNjcCI6InVzZXJfaW1wZXJzb25hdGlvbiIsImFjciI6IjEifQ.K7BCa0NO-A5f9exFiWcIXFMGnLmmt3V2HVP0itMT-GsAxnQROWzJFDIQNFo4QhiW0NCCqJykVELeVBCy_7Dex2-szUPZ69rmmDVJhy_qkmAiHhS1mNZDvJ1sB-whb5wOJ_QPIlByVzubhTcNnuliTVjnTeuOurVJJcn0Vugx9UDkGgky0etHXzmKukWYp4nzA68Wf1xnzlMZBz7PfoPGhjgzQfceOkZJVXIBRMB_7tsyW7gYNbHB_aTiT47cEjkh-UdrZEdp2UaAKugC-es3m076kRHMJqx31x-zDLDBttKinRJVPctiqwb1jMOMV6cUAp2E6aMfEbNk_iqX_OKFJg";
            var jwtEncodedString = tokenString;
            if (tokenString.Contains("Bearer"))
            {
                jwtEncodedString = tokenString.Substring(7); // trim 'Bearer ' from the start since its just a prefix for the token string
            }

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            string username = String.Empty;
            string name = String.Empty;
            string scope = String.Empty;
            string oid = String.Empty;
            List<string> roles = new List<string>();

            oid = token.Claims.FirstOrDefault(c => c.Type == "oid")?.Value;
            username = token.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            scope = token.Claims.FirstOrDefault(c => c.Type == "scp")?.Value;

            foreach (var item in token.Claims.Where(c => c.Type == "roles"))
            {
                roles.Add(item.Value);
            }

            var tokenInfo = new { username, oid, name, scope, roles };

            return new JsonResult(tokenInfo);
        }


        [AllowAnonymous]
        [HttpGet("auth-response")]
        public async Task<string> AuthResponse()
        {
            _logger.LogInformation("GetTokenByClientSecret called!");
            //var endPoint_post = "https://login.microsoftonline.com/9c822166-54c6-4c9c-b0eb-d390a8f54e66/oauth2/token";
            //var endPoint_get = "https://login.microsoftonline.com/9c822166-54c6-4c9c-b0eb-d390a8f54e66/oauth2/v2.0/authorize";

            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonSerializer.Serialize(""), Encoding.UTF8, "application/x-www-form-urlencoded");
                string endpoint = "https://login.microsoftonline.com/9c822166-54c6-4c9c-b0eb-d390a8f54e66/oauth2/v2.0/authorize?" +
                                    "client_id=ce996f11-8406-49fc-978e-06db61125cbe" +
                                    "&response_type=id_token" +
                                    "&redirect_uri=https://localhost/api/Auth/GetToken/" +
                                    "&scope=openid+profile+email" +
                                    "&response_mode=fragment" +
                                    "&state=12345" +
                                    "&nonce=678910";

                //using (var Response = await client.PostAsync(endpoint, content))
                using (var Response = await client.GetAsync(endpoint))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {


                    }
                }
            }

            return "";
        }

        [AllowAnonymous]
        [HttpGet("AnonymousAccess")]
        public async Task<IEnumerable<WeatherForecast>> AnonymousAccess()
        {
            _logger.LogInformation("AnonymousAccess: Weather Forecast called!");
            var rng = new Random();
            await Task.CompletedTask;
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }


    }

    public class TokenResponse
    {
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string ext_expires_in { get; set; }
        public string access_token { get; set; }
    }
}

