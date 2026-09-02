using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Endorsement.Queries.GetPolicyEndorsement
{
    public class GetPolicyEndorsementQuery : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("endorsementDate")]
        public string EndorsementDate { get; set; }
    }

    public class GetPolicyEndorsementQueryHandler : IRequestHandler<GetPolicyEndorsementQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetPolicyEndorsementQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public GetPolicyEndorsementQueryHandler(ILogger<GetPolicyEndorsementQueryHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetPolicyEndorsementQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetPolicyEndorsementQuery.Handle - In Process");

            var response = await _db2PolicyService.GetPolicyEndorsementAsync(request, cancellationToken);

            _logger.LogInformation("GetPolicyEndorsementQuery.Handle - Completed");
            return response;
        }
    }
}
