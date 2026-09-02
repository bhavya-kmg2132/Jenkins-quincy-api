using Application.Common.Interfaces;
using Application.ExternalPolicy.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.TaskManager.Commands.ReferAllTasks
{
    public class ReferAllTasksRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("refers")]
        public List<ReferTaskItem> Refers { get; set; }
    }

    public class ReferAllTasksRequestHandler : IRequestHandler<ReferAllTasksRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<ReferAllTasksRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public ReferAllTasksRequestHandler(ILogger<ReferAllTasksRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(ReferAllTasksRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ReferAllTasksRequest.Handle - In Process");

            var response = await _db2PolicyService.ReferAllTasksAsync(request, cancellationToken);

            _logger.LogInformation("ReferAllTasksRequest.Handle - Completed");
            return response;
        }
    }
}
