using Application.Common.Interfaces;
using Application.ExternalPolicy.Rules;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Policy.Commands.UpdateUnderwriterQuestions
{
    public class UpdateUnderwriterQuestionsRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("recordType")]
        public int? RecordType { get; set; }

        [JsonPropertyName("endorsementDate")]
        public string EndorsementDate { get; set; }

        [JsonPropertyName("safetyProgram")]
        public string SafetyProgram { get; set; }

        [JsonPropertyName("safetyProgramDescription")]
        public string SafetyProgramDescription { get; set; }

        [JsonPropertyName("flamable")]
        public string Flamable { get; set; }

        [JsonPropertyName("declineCancelNonRen")]
        public string DeclineCancelNonRen { get; set; }

        [JsonPropertyName("declineCancelNonRenDesc")]
        public string DeclineCancelNonRenDesc { get; set; }

        [JsonPropertyName("employee50")]
        public string Employee50 { get; set; }

        [JsonPropertyName("employee50Description")]
        public string Employee50Description { get; set; }

        [JsonPropertyName("maintenanceProgram")]
        public string MaintenanceProgram { get; set; }

        [JsonPropertyName("maintenanceProgramDescription")]
        public string MaintenanceProgramDescription { get; set; }

        [JsonPropertyName("filing")]
        public string Filing { get; set; }

        [JsonPropertyName("hazard")]
        public string Hazard { get; set; }

        [JsonPropertyName("mvrVerification")]
        public string MvrVerification { get; set; }

        [JsonPropertyName("mvrVerificationDesc")]
        public string MvrVerificationDesc { get; set; }

        [JsonPropertyName("deliverService")]
        public string DeliverService { get; set; }

        [JsonPropertyName("deliverServiceDesc")]
        public string DeliverServiceDesc { get; set; }

        [JsonPropertyName("deliverTimeLim")]
        public string DeliverTimeLim { get; set; }

        [JsonPropertyName("deliverTimeLimDesc")]
        public string DeliverTimeLimDesc { get; set; }

        [JsonPropertyName("personalAutoDel")]
        public string PersonalAutoDel { get; set; }

        [JsonPropertyName("personalAutoDelDesc")]
        public string PersonalAutoDelDesc { get; set; }

        [JsonPropertyName("snowPlowOrRemovalFee")]
        public string SnowPlowOrRemovalFee { get; set; }

        [JsonPropertyName("snowPlowOrRemFeeDesc")]
        public string SnowPlowOrRemFeeDesc { get; set; }

        [JsonPropertyName("nonEmpFamUseVeh")]
        public string NonEmpFamUseVeh { get; set; }

        [JsonPropertyName("nonEmpFamUseDrv")]
        public string NonEmpFamUseDrv { get; set; }

        [JsonPropertyName("nonEmpFamUseDrvDesc")]
        public string NonEmpFamUseDrvDesc { get; set; }

        [JsonPropertyName("primaryPolicy")]
        public string PrimaryPolicy { get; set; }

        [JsonPropertyName("priorCoverage")]
        public string PriorCoverage { get; set; }

        [JsonPropertyName("carrier")]
        public string Carrier { get; set; }

        [JsonPropertyName("priorExpirationDate")]
        public string PriorExpirationDate { get; set; }

        [JsonPropertyName("expiringPremium")]
        public double? ExpiringPremium { get; set; }

        [JsonPropertyName("threeYrsLossRatio")]
        public double? ThreeYrsLossRatio { get; set; }

        [JsonPropertyName("agentRemLn1")]
        public string AgentRemLn1 { get; set; }

        [JsonPropertyName("agentRemLn2")]
        public string AgentRemLn2 { get; set; }

        [JsonPropertyName("agentRemLn3")]
        public string AgentRemLn3 { get; set; }

        [JsonPropertyName("validFidNo")]
        public string ValidFidNo { get; set; }
    }

    public class UpdateUnderwriterQuestionsRequestHandler : IRequestHandler<UpdateUnderwriterQuestionsRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<UpdateUnderwriterQuestionsRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;
        private readonly IScreenFieldConditionRuleService _screenFieldConditionRuleService;

        public UpdateUnderwriterQuestionsRequestHandler(ILogger<UpdateUnderwriterQuestionsRequestHandler> logger, IDb2PolicyService db2PolicyService,
            IScreenFieldConditionRuleService screenFieldConditionRuleService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
            _screenFieldConditionRuleService = screenFieldConditionRuleService;
        }

        public async Task<ExternalPolicyResponse> Handle(UpdateUnderwriterQuestionsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateUnderwriterQuestionsRequest.Handle - In Process");

            //Rule Engine - screen field condition validations against the DB2 payload
            await _screenFieldConditionRuleService.Validate(ScreenRuleRequestMapper.ForUpdateUnderwriterQuestions(request));

            var response = await _db2PolicyService.UpdateUnderwriterQuestionsAsync(request, cancellationToken);

            _logger.LogInformation("UpdateUnderwriterQuestionsRequest.Handle - Completed");
            return response;
        }
    }
}
