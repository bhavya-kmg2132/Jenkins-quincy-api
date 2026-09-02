using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.PolicyCancellation.Commands.HoldTransaction
{
    public class HoldTransactionRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }
    }

    public class HoldTransactionRequestHandler : IRequestHandler<HoldTransactionRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<HoldTransactionRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public HoldTransactionRequestHandler(ILogger<HoldTransactionRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(HoldTransactionRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("HoldTransactionRequest.Handle - In Process");

            var response = await _db2PolicyService.HoldTransactionAsync(request, cancellationToken);

            _logger.LogInformation("HoldTransactionRequest.Handle - Completed");
            return response;
        }
    }
}
