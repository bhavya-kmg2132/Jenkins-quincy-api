using Application.Common.Interfaces;
using Application.ExternalPolicy.Models;
using Application.ExternalPolicy.Rules;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Vehicle.Commands.PatchVehicle
{
    public class PatchVehicleRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("vehicles")]
        public List<PatchVehicleItem> Vehicles { get; set; }

        [JsonPropertyName("common")]
        public PatchVehicleCommon Common { get; set; }
    }

    public class PatchVehicleRequestHandler : IRequestHandler<PatchVehicleRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<PatchVehicleRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;
        private readonly IScreenFieldConditionRuleService _screenFieldConditionRuleService;

        public PatchVehicleRequestHandler(ILogger<PatchVehicleRequestHandler> logger, IDb2PolicyService db2PolicyService,
            IScreenFieldConditionRuleService screenFieldConditionRuleService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
            _screenFieldConditionRuleService = screenFieldConditionRuleService;
        }

        public async Task<ExternalPolicyResponse> Handle(PatchVehicleRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("PatchVehicleRequest.Handle - In Process");

            //Rule Engine - screen field condition validations against the DB2 payload
            await _screenFieldConditionRuleService.Validate(ScreenRuleRequestMapper.ForPatchVehicle(request));

            var response = await _db2PolicyService.PatchVehicleAsync(request, cancellationToken);

            _logger.LogInformation("PatchVehicleRequest.Handle - Completed");
            return response;
        }
    }
}
