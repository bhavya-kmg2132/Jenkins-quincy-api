using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
// using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger _logger;
        private readonly ICurrentUserService _currentUserService;

        public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            string userName = string.Empty;

            var correlationId = _currentUserService.CorrelationId;
            var requestId = _currentUserService.RequestId;

            _logger.LogInformation("RequestInfo| RequestName:{@Name} | RequestId:{@requestId} | CorrelationId:{@correlationId} | RequestData:{@Request}",
                requestName, requestId, correlationId, request);

            return await next();

        }
    }
}