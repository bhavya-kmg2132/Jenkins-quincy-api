using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Notepad.Queries.GetNotepads
{
    public class GetNotepadsQuery : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
    }

    public class GetNotepadsQueryHandler : IRequestHandler<GetNotepadsQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetNotepadsQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public GetNotepadsQueryHandler(ILogger<GetNotepadsQueryHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetNotepadsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetNotepadsQuery.Handle - In Process");

            var response = await _db2PolicyService.GetNotepadsAsync(request, cancellationToken);

            _logger.LogInformation("GetNotepadsQuery.Handle - Completed");
            return response;
        }
    }
}
