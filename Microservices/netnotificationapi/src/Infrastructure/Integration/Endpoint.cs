using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Integration
{
    public class Endpoint : IEndpoint
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Endpoint> _logger;
        public string CorrelationId => string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-Id"]) ?
                                            Guid.NewGuid().ToString() : _httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-Id"];
        public string RequestId => string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Id"]) ?
                                            Guid.NewGuid().ToString() : _httpContextAccessor.HttpContext?.Request.Headers["X-Request-Id"];
        public string RequestOid => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Oid"] ?? string.Empty}";

        public string RequestUid => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Uid"] ?? string.Empty}";
        public string ApiKey => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Api-Key"] ?? string.Empty}";

        public Endpoint(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ILogger<Endpoint> logger)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._configuration = configuration;
            this._logger = logger;
        }

        public async Task<string> HttpGetRequestAsync(string serverURL, string apiEndpoint, string userName_userId_userOid)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(serverURL);
                    // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token");
                    client.DefaultRequestHeaders.Add("X-Correlation-Id", CorrelationId);
                    client.DefaultRequestHeaders.Add("X-Request-Id", RequestId);
                    client.DefaultRequestHeaders.Add("X-Request-Oid", userName_userId_userOid);
                    client.DefaultRequestHeaders.Add("X-Request-Uid", RequestUid);
                    client.DefaultRequestHeaders.Add("X-Api-Key", _configuration["InternalEndpoints:DataPulseApiKey"]);


                    HttpResponseMessage response = await client.GetAsync(apiEndpoint);
                    response.EnsureSuccessStatusCode(); // Throws exception for non-success status codes

                    return await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Error: " + ex.Message);
                }
            }
        }

        public string HttpGetRequestSync(string serverURL, string apiEndpoint)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(serverURL);
                    // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token");
                    client.DefaultRequestHeaders.Add("X-Correlation-Id", CorrelationId);
                    client.DefaultRequestHeaders.Add("X-Request-Id", RequestId);
                    client.DefaultRequestHeaders.Add("X-Request-Oid", RequestOid);
                    client.DefaultRequestHeaders.Add("X-Request-Uid", RequestUid);
                    client.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);

                    HttpResponseMessage response = client.GetAsync(apiEndpoint).Result;
                    response.EnsureSuccessStatusCode(); // Throws exception for non-success status codes

                    return response.Content.ReadAsStringAsync().Result;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Error: " + ex.Message);
                }
            }
        }

        public async Task<HttpResponseMessage> HttpPostRequest(string serverURL, string apiEnpoint, object postObject)
        {

            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) })
            {
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Add("X-Api-Key", _configuration["Api:api-key"]);
                client.DefaultRequestHeaders.Add("X-Request-Oid", "leeladhar.kumawat@kmgin.com");
                // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token");
                var serializeObject = JsonSerializer.Serialize(postObject);
                var content = new StringContent(serializeObject, Encoding.UTF8, "application/json");
                try
                {
                    var response = await client.PostAsync(apiEnpoint, content);
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Endpoint.HttpPostRequest:{ex.Message}");
                    throw;

                }
                //if (response.IsSuccessStatusCode)
                //{
                //    var responseContent = response.Content.ReadAsStringAsync().Result;
                //    return true; ;
                //}
                //else
                //{
                //    return false;
                //}
            }
        }

        public async Task<HttpResponseMessage> HttpBatchEmailPostRequest(string serverURL, string apiEndpoint, object postObject)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Ensure correct full URL
                    string fullUrl = new Uri(new Uri(serverURL), apiEndpoint).ToString();

                    // Headers for controller authentication
                    client.DefaultRequestHeaders.Add("X-Api-Key", _configuration["Api:api-key"]);

                    // Serialize JSON properly
                    var jsonRequest = JsonSerializer.Serialize(postObject);
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    // Logging request for debugging
                    Console.WriteLine("Request JSON: " + jsonRequest);

                    var response = await client.PostAsync(fullUrl, content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Log response for debugging
                    Console.WriteLine("Response: " + responseContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"HTTP POST Failed: {response.StatusCode} - {responseContent}");
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error in HttpPostRequest: {ex.Message}", ex);
                }
            }
        }
    }
}