using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Policy.Queries.GetPolicyDetail
{
    public class GetPolicyDetailQuery : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }
    }

    public class GetPolicyDetailQueryHandler : IRequestHandler<GetPolicyDetailQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetPolicyDetailQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public GetPolicyDetailQueryHandler(ILogger<GetPolicyDetailQueryHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetPolicyDetailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetPolicyDetailQuery.Handle - In Process");

            var response = await _db2PolicyService.GetPolicyDetailAsync(request, cancellationToken);

            _logger.LogInformation("GetPolicyDetailQuery.Handle - Completed");
            return response;
        }
    }
}
