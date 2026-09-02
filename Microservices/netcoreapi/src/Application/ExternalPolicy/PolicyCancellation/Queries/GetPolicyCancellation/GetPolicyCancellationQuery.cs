using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.PolicyCancellation.Queries.GetPolicyCancellation
{
    public class GetPolicyCancellationQuery : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("userIdAdd")]
        public string UserIdAdd { get; set; }

        [JsonPropertyName("cancelDate")]
        public string CancelDate { get; set; }
    }

    public class GetPolicyCancellationQueryHandler : IRequestHandler<GetPolicyCancellationQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetPolicyCancellationQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public GetPolicyCancellationQueryHandler(ILogger<GetPolicyCancellationQueryHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetPolicyCancellationQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetPolicyCancellationQuery.Handle - In Process");

            var response = await _db2PolicyService.GetPolicyCancellationAsync(request, cancellationToken);

            _logger.LogInformation("GetPolicyCancellationQuery.Handle - Completed");
            return response;
        }
    }
}
