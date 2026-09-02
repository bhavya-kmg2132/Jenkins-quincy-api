using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.TaskManager.Commands.ReopenTask
{
    public class ReopenTaskRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("taskCode")]
        public string TaskCode { get; set; }

        [JsonPropertyName("sequenceNumber")]
        public int? SequenceNumber { get; set; }

        [JsonPropertyName("userIdAdd")]
        public string UserIdAdd { get; set; }
    }

    public class ReopenTaskRequestHandler : IRequestHandler<ReopenTaskRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<ReopenTaskRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public ReopenTaskRequestHandler(ILogger<ReopenTaskRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(ReopenTaskRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ReopenTaskRequest.Handle - In Process");

            var response = await _db2PolicyService.ReopenTaskAsync(request, cancellationToken);

            _logger.LogInformation("ReopenTaskRequest.Handle - Completed");
            return response;
        }
    }
}
