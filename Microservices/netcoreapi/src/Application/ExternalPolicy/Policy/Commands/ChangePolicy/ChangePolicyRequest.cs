using Application.Common.Interfaces;
using Application.ExternalPolicy.Rules;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Policy.Commands.ChangePolicy
{
    public class ChangePolicyRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }
    }

    public class ChangePolicyRequestHandler : IRequestHandler<ChangePolicyRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<ChangePolicyRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public ChangePolicyRequestHandler(ILogger<ChangePolicyRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(ChangePolicyRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ChangePolicyRequest.Handle - In Process");

            var response = await _db2PolicyService.ChangePolicyAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                // DB2 returns OBILMD as a thousands-scale code (e.g. 100 for a $100,000 limit); scale it back up.
                response.Content = OptionalBodilyInjuryLimitScaler.ScaleUpFromDb2(response.Content);
            }

            _logger.LogInformation("ChangePolicyRequest.Handle - Completed");
            return response;
        }
    }
}
