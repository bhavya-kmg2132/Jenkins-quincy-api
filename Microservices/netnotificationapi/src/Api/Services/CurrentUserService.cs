using System;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CurrentUserService> _logger;
        private readonly IConfiguration _configuration;

        public string CorrelationId => string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-Id"]) ?
                                            Guid.NewGuid().ToString() : _httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-Id"];
        public string RequestId => string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Request.Headers["X-Request-Id"]) ?
                                            Guid.NewGuid().ToString() : _httpContextAccessor.HttpContext?.Request.Headers["X-Request-Id"];
        public string ApiKey => $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Api-Key"] ?? string.Empty}";

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this._httpContextAccessor = httpContextAccessor;
            this._configuration = configuration;
            ValidateApiKey();
        }

        public void ValidateApiKey()
        {
            string apiKey = this._configuration["Api:api-key"];

            if (!string.IsNullOrEmpty(apiKey))
            {
                //string Request_X_Api_Key = $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Api-Key"] ?? string.Empty}";

                if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
                {
                    _logger.LogError("Error CurrentUserService-ValidateApiKey: Valid API key is missing in request!");
                    throw new UnauthorizedAccessException("Error CurrentUserService-ValidateApiKey: Valid API key is missing in request!");
                }

                //var appSettings = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

                if (!apiKey.Equals(extractedApiKey))
                {
                    _logger.LogError("Error CurrentUserService-ValidateApiKey: Unauthorized client! Invalid Api Key!");
                    throw new UnauthorizedAccessException("Error CurrentUserService-ValidateApiKey: Unauthorized client! Invalid Api Key!");
                }
            }
        }
    }
}
