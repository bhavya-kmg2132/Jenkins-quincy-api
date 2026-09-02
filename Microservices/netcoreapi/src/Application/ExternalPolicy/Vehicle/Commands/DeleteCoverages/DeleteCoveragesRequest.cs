using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Vehicle.Commands.DeleteCoverages
{
    public class DeleteCoveragesRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }
    }

    public class DeleteCoveragesRequestHandler : IRequestHandler<DeleteCoveragesRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<DeleteCoveragesRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public DeleteCoveragesRequestHandler(ILogger<DeleteCoveragesRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(DeleteCoveragesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteCoveragesRequest.Handle - In Process");

            var response = await _db2PolicyService.DeleteCoveragesAsync(request, cancellationToken);

            _logger.LogInformation("DeleteCoveragesRequest.Handle - Completed");
            return response;
        }
    }
}
