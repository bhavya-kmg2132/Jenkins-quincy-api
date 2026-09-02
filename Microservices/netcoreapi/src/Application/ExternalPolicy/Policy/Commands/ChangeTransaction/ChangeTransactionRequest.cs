using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Policy.Commands.ChangeTransaction
{
    public class ChangeTransactionRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }
    }

    public class ChangeTransactionRequestHandler : IRequestHandler<ChangeTransactionRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<ChangeTransactionRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public ChangeTransactionRequestHandler(ILogger<ChangeTransactionRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(ChangeTransactionRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ChangeTransactionRequest.Handle - In Process");

            var response = await _db2PolicyService.ChangeTransactionAsync(request, cancellationToken);

            _logger.LogInformation("ChangeTransactionRequest.Handle - Completed");
            return response;
        }
    }
}
