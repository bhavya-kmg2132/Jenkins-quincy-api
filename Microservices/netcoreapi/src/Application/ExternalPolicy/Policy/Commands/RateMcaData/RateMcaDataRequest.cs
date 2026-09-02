using Application.Common.Interfaces;
using Application.ExternalPolicy.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Policy.Commands.RateMcaData
{
    public class RateMcaDataRequest : IRequest<ExternalPolicyResponse>
    {
        public List<PolicyDataTable> PolicyData { get; set; }
    }

    public class RateMcaDataRequestHandler : IRequestHandler<RateMcaDataRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<RateMcaDataRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;
        private readonly IScreenFieldConditionRuleService _screenFieldConditionRuleService;

        public RateMcaDataRequestHandler(ILogger<RateMcaDataRequestHandler> logger, IDb2PolicyService db2PolicyService,
            IScreenFieldConditionRuleService screenFieldConditionRuleService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
            _screenFieldConditionRuleService = screenFieldConditionRuleService;
        }

        public async Task<ExternalPolicyResponse> Handle(RateMcaDataRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("RateMcaDataRequest.Handle - In Process");

            //Rule Engine - screen field condition validations against the DB2 payload
            await _screenFieldConditionRuleService.Validate(request.PolicyData);

            var response = await _db2PolicyService.RateMcaDataAsync(request.PolicyData, cancellationToken);

            _logger.LogInformation("RateMcaDataRequest.Handle - Completed");
            return response;
        }
    }
}
