using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Vehicle.Queries.GetVehicleLocationMaster
{
    public class GetVehicleLocationMasterQuery : IRequest<ExternalPolicyResponse>
    {
    }

    public class GetVehicleLocationMasterQueryHandler : IRequestHandler<GetVehicleLocationMasterQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetVehicleLocationMasterQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public GetVehicleLocationMasterQueryHandler(ILogger<GetVehicleLocationMasterQueryHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetVehicleLocationMasterQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetVehicleLocationMasterQuery.Handle - In Process");

            var response = await _db2PolicyService.GetVehicleLocationMasterAsync(cancellationToken);

            _logger.LogInformation("GetVehicleLocationMasterQuery.Handle - Completed");
            return response;
        }
    }
}
