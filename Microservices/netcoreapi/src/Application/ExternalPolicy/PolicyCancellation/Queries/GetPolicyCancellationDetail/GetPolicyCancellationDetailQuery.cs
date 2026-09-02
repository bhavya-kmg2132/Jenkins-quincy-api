using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.PolicyCancellation.Queries.GetPolicyCancellationDetail
{
    public class GetPolicyCancellationDetailQuery : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("endorsementNumber")]
        public int EndorsementNumber { get; set; }

        [JsonPropertyName("cancellationReason")]
        public string CancellationReason { get; set; }

        [JsonPropertyName("cancellationDescription")]
        public string CancellationDescription { get; set; }

        [JsonPropertyName("cancellationCarrier")]
        public string CancellationCarrier { get; set; }

        [JsonPropertyName("policyRetainedByAgency")]
        public string PolicyRetainedByAgency { get; set; }

        [JsonPropertyName("cancelMethod")]
        public string CancelMethod { get; set; }
    }

    public class GetPolicyCancellationDetailQueryHandler : IRequestHandler<GetPolicyCancellationDetailQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetPolicyCancellationDetailQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public GetPolicyCancellationDetailQueryHandler(ILogger<GetPolicyCancellationDetailQueryHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetPolicyCancellationDetailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetPolicyCancellationDetailQuery.Handle - In Process");

            var response = await _db2PolicyService.GetPolicyCancellationDetailAsync(request, cancellationToken);

            _logger.LogInformation("GetPolicyCancellationDetailQuery.Handle - Completed");
            return response;
        }
    }
}
