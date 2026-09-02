using Gateway.Interfaces;

namespace Gateway.Integration
{
    public class Endpoint : IEndpoint
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public string CorrelationId => string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-Id"]) ?
                                            Guid.NewGuid().ToString() : _httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-Id"];
        public string RequestId => string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Id"]) ?
                                            Guid.NewGuid().ToString() : _httpContextAccessor.HttpContext?.Request.Headers["X-Request-Id"];
        public string RequestOid => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Oid"] ?? string.Empty}";

        public string RequestUid => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Uid"] ?? string.Empty}";
        public string ApiKey => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Api-Key"] ?? string.Empty}";

        public Endpoint(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> HttpGetRequestAsync(string serverURL, string apiEndpoint)
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

    }
}