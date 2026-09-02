using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Policy.Queries.GetQuotes
{
    public class GetQuotesQuery : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("insuredName")]
        public string InsuredName { get; set; }

        [JsonPropertyName("agentCode")]
        public string AgentCode { get; set; }

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
    }

    public class GetQuotesQueryHandler : IRequestHandler<GetQuotesQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetQuotesQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public GetQuotesQueryHandler(ILogger<GetQuotesQueryHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetQuotesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetQuotesQuery.Handle - In Process");

            var response = await _db2PolicyService.GetQuotesAsync(request, cancellationToken);

            _logger.LogInformation("GetQuotesQuery.Handle - Completed");
            return response;
        }
    }
}
