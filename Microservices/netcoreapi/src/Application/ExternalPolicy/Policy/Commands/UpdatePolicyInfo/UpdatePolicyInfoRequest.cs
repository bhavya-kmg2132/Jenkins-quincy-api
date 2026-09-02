using Application.Common.Interfaces;
using Application.ExternalPolicy.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Policy.Commands.UpdatePolicyInfo
{
    public class UpdatePolicyInfoRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("action")]
        public List<string> Action { get; set; }

        [JsonPropertyName("policyData")]
        public List<PolicyDataTable> PolicyData { get; set; }
    }

    public class UpdatePolicyInfoRequestHandler : IRequestHandler<UpdatePolicyInfoRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<UpdatePolicyInfoRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;
        private readonly IScreenFieldConditionRuleService _screenFieldConditionRuleService;

        public UpdatePolicyInfoRequestHandler(ILogger<UpdatePolicyInfoRequestHandler> logger, IDb2PolicyService db2PolicyService,
            IScreenFieldConditionRuleService screenFieldConditionRuleService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
            _screenFieldConditionRuleService = screenFieldConditionRuleService;
        }

        public async Task<ExternalPolicyResponse> Handle(UpdatePolicyInfoRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdatePolicyInfoRequest.Handle - In Process");

            //Rule Engine - screen field condition validations against the DB2 payload
            await _screenFieldConditionRuleService.Validate(request.PolicyData);

            var response = await _db2PolicyService.UpdatePolicyInfoAsync(request, cancellationToken);

            _logger.LogInformation("UpdatePolicyInfoRequest.Handle - Completed");
            return response;
        }
    }
}
