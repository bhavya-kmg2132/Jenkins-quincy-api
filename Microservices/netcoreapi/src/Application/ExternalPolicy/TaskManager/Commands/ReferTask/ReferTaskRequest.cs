using Application.Common.Interfaces;
using Application.ExternalPolicy.Models;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.TaskManager.Commands.ReferTask
{
    public class ReferTaskRequest : ReferTaskItem, IRequest<ExternalPolicyResponse>
    {
    }

    public class ReferTaskRequestHandler : IRequestHandler<ReferTaskRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<ReferTaskRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public ReferTaskRequestHandler(ILogger<ReferTaskRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(ReferTaskRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ReferTaskRequest.Handle - In Process");

            var response = await _db2PolicyService.ReferTaskAsync(request, cancellationToken);

            _logger.LogInformation("ReferTaskRequest.Handle - Completed");
            return response;
        }
    }
}
