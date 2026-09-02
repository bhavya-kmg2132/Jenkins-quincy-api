using Application.Common.Interfaces;
using Application.ExternalPolicy.Models;
using Application.ExternalPolicy.Rules;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Vehicle.Commands.AddVehicle
{
    public class AddVehicleRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }

        [JsonPropertyName("vinRequest")]
        public List<AddVinItemRequest> VinRequest { get; set; }

        [JsonPropertyName("registrationRequest")]
        public List<AddRegistrationRequest> RegistrationRequest { get; set; }
    }

    public class AddVehicleRequestHandler : IRequestHandler<AddVehicleRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<AddVehicleRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;
        private readonly IScreenFieldConditionRuleService _screenFieldConditionRuleService;

        public AddVehicleRequestHandler(ILogger<AddVehicleRequestHandler> logger, IDb2PolicyService db2PolicyService,
            IScreenFieldConditionRuleService screenFieldConditionRuleService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
            _screenFieldConditionRuleService = screenFieldConditionRuleService;
        }

        public async Task<ExternalPolicyResponse> Handle(AddVehicleRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddVehicleRequest.Handle - In Process");

            //Rule Engine - screen field condition validations against the DB2 payload
            await _screenFieldConditionRuleService.Validate(ScreenRuleRequestMapper.ForAddVehicle(request));

            var response = await _db2PolicyService.AddVehicleAsync(request, cancellationToken);

            _logger.LogInformation("AddVehicleRequest.Handle - Completed");
            return response;
        }
    }
}
