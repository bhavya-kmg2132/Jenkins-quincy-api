using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Vehicle.Queries.GetVehicleDetail
{
    public class GetVehicleDetailQuery : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("vehicleId")]
        public string VehicleId { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }
    }

    public class GetVehicleDetailQueryHandler : IRequestHandler<GetVehicleDetailQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetVehicleDetailQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public GetVehicleDetailQueryHandler(ILogger<GetVehicleDetailQueryHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetVehicleDetailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetVehicleDetailQuery.Handle - In Process");

            var response = await _db2PolicyService.GetVehicleDetailAsync(request, cancellationToken);

            _logger.LogInformation("GetVehicleDetailQuery.Handle - Completed");
            return response;
        }
    }
}
