using Application.Common.Interfaces;
using Application.ExternalPolicy.Models;
using Application.ExternalPolicy.Rules;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Driver.Commands.PatchDriver
{
    public class PatchDriverRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("drivers")]
        public List<PatchDriverItem> Drivers { get; set; }
    }

    public class PatchDriverRequestHandler : IRequestHandler<PatchDriverRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<PatchDriverRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;
        private readonly IScreenFieldConditionRuleService _screenFieldConditionRuleService;

        public PatchDriverRequestHandler(ILogger<PatchDriverRequestHandler> logger, IDb2PolicyService db2PolicyService,
            IScreenFieldConditionRuleService screenFieldConditionRuleService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
            _screenFieldConditionRuleService = screenFieldConditionRuleService;
        }

        public async Task<ExternalPolicyResponse> Handle(PatchDriverRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("PatchDriverRequest.Handle - In Process");

            //Rule Engine - screen field condition validations against the DB2 payload
            await _screenFieldConditionRuleService.Validate(ScreenRuleRequestMapper.ForPatchDriver(request));

            var response = await _db2PolicyService.PatchDriverAsync(request, cancellationToken);

            _logger.LogInformation("PatchDriverRequest.Handle - Completed");
            return response;
        }
    }
}
