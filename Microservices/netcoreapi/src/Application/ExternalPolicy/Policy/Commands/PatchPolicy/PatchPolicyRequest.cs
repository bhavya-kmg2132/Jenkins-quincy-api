using Application.Common.Interfaces;
using Application.ExternalPolicy.Rules;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Policy.Commands.PatchPolicy
{
    public class PatchPolicyRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("insuredName")]
        public string InsuredName { get; set; }

        [JsonPropertyName("addlInsName")]
        public List<string> AddlInsName { get; set; }

        [JsonPropertyName("insuredType")]
        public string InsuredType { get; set; }

        [JsonPropertyName("licenseNumber")]
        public string LicenseNumber { get; set; }

        [JsonPropertyName("insuredAddress1")]
        public string InsuredAddress1 { get; set; }

        [JsonPropertyName("insuredAddress2")]
        public string InsuredAddress2 { get; set; }

        [JsonPropertyName("insuredCity")]
        public string InsuredCity { get; set; }

        [JsonPropertyName("insuredState")]
        public string InsuredState { get; set; }

        [JsonPropertyName("insuredZip")]
        public string InsuredZip { get; set; }

        [JsonPropertyName("insuredTelephone")]
        public string InsuredTelephone { get; set; }

        [JsonPropertyName("phoneType")]
        public string PhoneType { get; set; }

        [JsonPropertyName("secondEmailId")]
        public string SecondEmailId { get; set; }

        [JsonPropertyName("contact")]
        public string Contact { get; set; }

        [JsonPropertyName("relatedPolicy")]
        public string RelatedPolicy { get; set; }

        [JsonPropertyName("account")]
        public string Account { get; set; }

        [JsonPropertyName("enhancementCoverage")]
        public string EnhancementCoverage { get; set; }
    }

    public class PatchPolicyRequestHandler : IRequestHandler<PatchPolicyRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<PatchPolicyRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;
        private readonly IScreenFieldConditionRuleService _screenFieldConditionRuleService;

        public PatchPolicyRequestHandler(ILogger<PatchPolicyRequestHandler> logger, IDb2PolicyService db2PolicyService,
            IScreenFieldConditionRuleService screenFieldConditionRuleService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
            _screenFieldConditionRuleService = screenFieldConditionRuleService;
        }

        public async Task<ExternalPolicyResponse> Handle(PatchPolicyRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("PatchPolicyRequest.Handle - In Process");

            //Rule Engine - screen field condition validations against the DB2 payload
            await _screenFieldConditionRuleService.Validate(ScreenRuleRequestMapper.ForPatchPolicy(request));

            var response = await _db2PolicyService.PatchPolicyAsync(request, cancellationToken);

            _logger.LogInformation("PatchPolicyRequest.Handle - Completed");
            return response;
        }
    }
}
