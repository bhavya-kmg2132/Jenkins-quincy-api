using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.PolicyCancellation.Commands.CancelPolicy
{
    public class CancelPolicyRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("endorsementNumber")]
        public int EndorsementNumber { get; set; }

        [JsonPropertyName("cancelDate")]
        public string CancelDate { get; set; }
    }

    public class CancelPolicyRequestHandler : IRequestHandler<CancelPolicyRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<CancelPolicyRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public CancelPolicyRequestHandler(ILogger<CancelPolicyRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(CancelPolicyRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CancelPolicyRequest.Handle - In Process");

            var response = await _db2PolicyService.CancelPolicyAsync(request, cancellationToken);

            _logger.LogInformation("CancelPolicyRequest.Handle - Completed");
            return response;
        }
    }
}
