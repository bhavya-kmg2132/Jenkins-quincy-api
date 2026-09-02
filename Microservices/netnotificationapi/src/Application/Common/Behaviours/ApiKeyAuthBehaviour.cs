using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours
{
    public class ApiKeyAuthBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TRequest> _logger;

        public ApiKeyAuthBehaviour(IHttpContextAccessor httpContextAccessor, ILogger<TRequest> logger, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _configuration = configuration;
        }


        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            string apiKey = this._configuration["Api:api-key"];

            if (!string.IsNullOrEmpty(apiKey))
            {
                //string Request_X_Api_Key = $"{_httpContextAccessor.HttpContext?.Request.Headers["X-Api-Key"] ?? string.Empty}";

                if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
                {
                    _logger.LogError("Valid API key is missing in request!");
                    throw new UnauthorizedAccessException("Valid API key is missing in request!");
                }

                //var appSettings = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

                if (!apiKey.Equals(extractedApiKey))
                {
                    _logger.LogError("Unauthorized client! Invalid Api Key!");
                    throw new UnauthorizedAccessException("Unauthorized client! Invalid Api Key!");
                }
            }
            // User is authorized or authorization not required
            return await next();
        }

    }
}
