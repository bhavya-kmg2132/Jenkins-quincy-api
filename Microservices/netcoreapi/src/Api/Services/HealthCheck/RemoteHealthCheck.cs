using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.Services.HealthCheck
{
    public class RemoteHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IConfiguration _configuration;
        public RemoteHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                //var response = await httpClient.GetAsync("https://api.ipify.org");
                var response = await httpClient.GetAsync(string.Concat(_configuration["InternalEndpoints:netauthapi"], "api/User/GetUserByUserName?userName=manish@signat.com"));
                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy(_configuration["InternalEndpoints:netauthapi"] + " - " + $"Remote endpoints is healthy.");
                }

                return HealthCheckResult.Unhealthy(_configuration["InternalEndpoints:netauthapi"] + " - " + "Remote endpoint is unhealthy");
            }
        }
    }
}
