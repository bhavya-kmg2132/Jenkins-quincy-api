using Application.Common.Interfaces;
using Application.ExternalPolicy.Models;
using Application.ExternalPolicy.Rules;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Driver.Commands.AddDriver
{
    public class AddDriverRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("driverRequest")]
        public List<DriverLicenseRequest> DriverRequest { get; set; }
    }

    public class AddDriverRequestHandler : IRequestHandler<AddDriverRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<AddDriverRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;
        private readonly IScreenFieldConditionRuleService _screenFieldConditionRuleService;

        public AddDriverRequestHandler(ILogger<AddDriverRequestHandler> logger, IDb2PolicyService db2PolicyService,
            IScreenFieldConditionRuleService screenFieldConditionRuleService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
            _screenFieldConditionRuleService = screenFieldConditionRuleService;
        }

        public async Task<ExternalPolicyResponse> Handle(AddDriverRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddDriverRequest.Handle - In Process");

            //Rule Engine - screen field condition validations against the DB2 payload
            await _screenFieldConditionRuleService.Validate(ScreenRuleRequestMapper.ForAddDriver(request));

            var response = await _db2PolicyService.AddDriverAsync(request, cancellationToken);

            _logger.LogInformation("AddDriverRequest.Handle - Completed");
            return response;
        }
    }
}
