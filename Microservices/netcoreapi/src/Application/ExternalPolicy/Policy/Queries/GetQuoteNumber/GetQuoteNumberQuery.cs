using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Policy.Queries.GetQuoteNumber
{
    public class GetQuoteNumberQuery : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("winsProductCode")]
        public string WinsProductCode { get; set; }

        [JsonPropertyName("subSystem")]
        public string SubSystem { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("idChangeExtended")]
        public string IdChangeExtended { get; set; }
    }

    public class GetQuoteNumberQueryHandler : IRequestHandler<GetQuoteNumberQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetQuoteNumberQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;
        private readonly ICurrentUserService _currentUserService;

        public GetQuoteNumberQueryHandler(ILogger<GetQuoteNumberQueryHandler> logger, IDb2PolicyService db2PolicyService,
            ICurrentUserService currentUserService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
            _currentUserService = currentUserService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetQuoteNumberQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetQuoteNumberQuery.Handle - In Process");

            if (string.IsNullOrWhiteSpace(request.IdChangeExtended))
                request.IdChangeExtended = _currentUserService.UserName;

            var response = await _db2PolicyService.GetQuoteNumberAsync(request, cancellationToken);

            _logger.LogInformation("GetQuoteNumberQuery.Handle - Completed");
            return response;
        }
    }
}
