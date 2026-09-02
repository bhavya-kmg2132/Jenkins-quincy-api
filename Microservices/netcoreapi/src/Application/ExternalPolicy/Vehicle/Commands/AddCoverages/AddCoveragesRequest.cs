using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Vehicle.Commands.AddCoverages
{
    public class AddCoveragesRequest : IRequest<ExternalPolicyResponse>
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

    public class AddCoveragesRequestHandler : IRequestHandler<AddCoveragesRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<AddCoveragesRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public AddCoveragesRequestHandler(ILogger<AddCoveragesRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(AddCoveragesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddCoveragesRequest.Handle - In Process");

            var response = await _db2PolicyService.AddCoveragesAsync(request, cancellationToken);

            _logger.LogInformation("AddCoveragesRequest.Handle - Completed");
            return response;
        }
    }
}
