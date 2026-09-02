using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Driver.Queries.GetDriverDetail
{
    public class GetDriverDetailQuery : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("licenseNumber")]
        public string LicenseNumber { get; set; }
    }

    public class GetDriverDetailQueryHandler : IRequestHandler<GetDriverDetailQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetDriverDetailQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public GetDriverDetailQueryHandler(ILogger<GetDriverDetailQueryHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetDriverDetailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetDriverDetailQuery.Handle - In Process");

            var response = await _db2PolicyService.GetDriverDetailAsync(request, cancellationToken);

            _logger.LogInformation("GetDriverDetailQuery.Handle - Completed");
            return response;
        }
    }
}
