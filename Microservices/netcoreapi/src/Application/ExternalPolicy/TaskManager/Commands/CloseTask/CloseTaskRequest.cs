using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.TaskManager.Commands.CloseTask
{
    public class CloseTaskRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("followUpDate")]
        public string FollowUpDate { get; set; }

        [JsonPropertyName("referralReason")]
        public string ReferralReason { get; set; }

        [JsonPropertyName("referralComment")]
        public string ReferralComment { get; set; }

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

    public class CloseTaskRequestHandler : IRequestHandler<CloseTaskRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<CloseTaskRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public CloseTaskRequestHandler(ILogger<CloseTaskRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(CloseTaskRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CloseTaskRequest.Handle - In Process");

            var response = await _db2PolicyService.CloseTaskAsync(request, cancellationToken);

            _logger.LogInformation("CloseTaskRequest.Handle - Completed");
            return response;
        }
    }
}
