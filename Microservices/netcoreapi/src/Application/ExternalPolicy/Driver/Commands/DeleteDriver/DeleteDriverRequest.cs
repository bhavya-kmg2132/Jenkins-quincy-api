using Application.Common.Interfaces;
using Application.ExternalPolicy.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Driver.Commands.DeleteDriver
{
    public class DeleteDriverRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("drivers")]
        public List<DeleteDriverItem> Drivers { get; set; }
    }

    public class DeleteDriverRequestHandler : IRequestHandler<DeleteDriverRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<DeleteDriverRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public DeleteDriverRequestHandler(ILogger<DeleteDriverRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(DeleteDriverRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteDriverRequest.Handle - In Process");

            var response = await _db2PolicyService.DeleteDriverAsync(request, cancellationToken);

            _logger.LogInformation("DeleteDriverRequest.Handle - Completed");
            return response;
        }
    }
}
