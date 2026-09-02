using Application.Common.Interfaces;
using Application.ExternalPolicy.Models;
using Application.ExternalPolicy.Rules;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Policy.Commands.SavePolicyInfo
{
    public class SavePolicyInfoRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("insured")]
        public Insured Insured { get; set; }

        [JsonPropertyName("telephone")]
        public string Telephone { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("mailingAddress")]
        public Address MailingAddress { get; set; }

        [JsonPropertyName("coverageIndicators")]
        public CoverageIndicators CoverageIndicators { get; set; }

        [JsonPropertyName("underwriterQuestions")]
        public UnderwriterQuestions UnderwriterQuestions { get; set; }
    }

    public class SavePolicyInfoRequestHandler : IRequestHandler<SavePolicyInfoRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<SavePolicyInfoRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;
        private readonly IScreenFieldConditionRuleService _screenFieldConditionRuleService;

        public SavePolicyInfoRequestHandler(ILogger<SavePolicyInfoRequestHandler> logger, IDb2PolicyService db2PolicyService,
            IScreenFieldConditionRuleService screenFieldConditionRuleService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
            _screenFieldConditionRuleService = screenFieldConditionRuleService;
        }

        public async Task<ExternalPolicyResponse> Handle(SavePolicyInfoRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("SavePolicyInfoRequest.Handle - In Process");

            //Rule Engine - screen field condition validations against the DB2 payload
            await _screenFieldConditionRuleService.Validate(ScreenRuleRequestMapper.ForSavePolicyInfo(request));

            var response = await _db2PolicyService.SavePolicyInfoAsync(request, cancellationToken);

            _logger.LogInformation("SavePolicyInfoRequest.Handle - Completed");
            return response;
        }
    }
}
