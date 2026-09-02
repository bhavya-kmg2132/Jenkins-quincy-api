using Application.Common.Interfaces;
using Application.ExternalPolicy.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Vehicle.Commands.DeleteVehicle
{
    public class DeleteVehicleRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("vehicles")]
        public List<DeleteVehicleItemRequest> Vehicles { get; set; }
    }

    public class DeleteVehicleRequestHandler : IRequestHandler<DeleteVehicleRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<DeleteVehicleRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public DeleteVehicleRequestHandler(ILogger<DeleteVehicleRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(DeleteVehicleRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteVehicleRequest.Handle - In Process");

            var response = await _db2PolicyService.DeleteVehicleAsync(request, cancellationToken);

            _logger.LogInformation("DeleteVehicleRequest.Handle - Completed");
            return response;
        }
    }
}
