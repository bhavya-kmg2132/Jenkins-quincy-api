using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Policy.Queries.GetPolicyHistory
{
    public class GetPolicyHistoryQuery : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
    }

    public class GetPolicyHistoryQueryHandler : IRequestHandler<GetPolicyHistoryQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetPolicyHistoryQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public GetPolicyHistoryQueryHandler(ILogger<GetPolicyHistoryQueryHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetPolicyHistoryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetPolicyHistoryQuery.Handle - In Process");

            var response = await _db2PolicyService.GetPolicyHistoryAsync(request, cancellationToken);

            _logger.LogInformation("GetPolicyHistoryQuery.Handle - Completed");
            return response;
        }
    }
}
