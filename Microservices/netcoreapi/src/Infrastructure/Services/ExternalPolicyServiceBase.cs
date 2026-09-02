using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public abstract class ExternalPolicyServiceBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger _logger;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        protected ExternalPolicyServiceBase(HttpClient httpClient, IConfiguration configuration, ICurrentUserService currentUserService, ILogger logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        protected Task<ExternalPolicyResponse> GetAsync(string path, CancellationToken cancellationToken)
            => SendAsync(path, HttpMethod.Get, null, cancellationToken);

        protected Task<ExternalPolicyResponse> PostAsync(string path, object body, CancellationToken cancellationToken)
            => SendAsync(path, HttpMethod.Post, body, cancellationToken);

        protected Task<ExternalPolicyResponse> PutAsync(string path, object body, CancellationToken cancellationToken)
            => SendAsync(path, HttpMethod.Put, body, cancellationToken);

        protected Task<ExternalPolicyResponse> PatchAsync(string path, object body, CancellationToken cancellationToken)
            => SendAsync(path, HttpMethod.Patch, body, cancellationToken);

        protected Task<ExternalPolicyResponse> DeleteAsync(string path, object body, CancellationToken cancellationToken)
            => SendAsync(path, HttpMethod.Delete, body, cancellationToken);

        private async Task<ExternalPolicyResponse> SendAsync(
            string path,
            HttpMethod method,
            object body,
            CancellationToken cancellationToken)
        {
            var baseUrl = _configuration["Db2Api:BaseUrl"]
                ?? throw new InvalidOperationException("Missing Db2Api:BaseUrl configuration.");

            var url = new Uri(new Uri(baseUrl), path).ToString();
            using var request = new HttpRequestMessage(method, url);

            if (body != null)
            {
                var json = JsonSerializer.Serialize(body, JsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var apiKey = _configuration["Db2Api:ApiKey"];
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                request.Headers.Add("x-api-key", apiKey);
            }

            var correlationId = _currentUserService?.CorrelationId;
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                request.Headers.Add("X-Correlation-Id", correlationId);
            }

            _logger.LogInformation("Calling DB2 policy API: {Method} {Url} | X-Correlation-Id: {CorrelationId}",
                method, url, correlationId ?? "(none)");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("DB2 policy API returned {StatusCode}. Response: {Response}",
                    (int)response.StatusCode, content);
            }

            return new ExternalPolicyResponse
            {
                StatusCode = (int)response.StatusCode,
                Content = content,
                IsSuccessStatusCode = response.IsSuccessStatusCode
            };
        }
    }
}
