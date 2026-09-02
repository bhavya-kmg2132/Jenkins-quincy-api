using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.PolicyCancellation.Commands.DeleteTransPolicy
{
    public class DeleteTransPolicyRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("isBackClick")]
        public bool IsBackClick { get; set; }
    }

    public class DeleteTransPolicyRequestHandler : IRequestHandler<DeleteTransPolicyRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<DeleteTransPolicyRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public DeleteTransPolicyRequestHandler(ILogger<DeleteTransPolicyRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(DeleteTransPolicyRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteTransPolicyRequest.Handle - In Process");

            var response = await _db2PolicyService.DeleteTransPolicyAsync(request, cancellationToken);

            _logger.LogInformation("DeleteTransPolicyRequest.Handle - Completed");
            return response;
        }
    }
}
