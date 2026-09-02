using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.TaskManager.Commands.UpdateTask
{
    public class UpdateTaskRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("paymentDueDate")]
        public string PaymentDueDate { get; set; }

        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("taskCode")]
        public string TaskCode { get; set; }

        [JsonPropertyName("sequenceNumber")]
        public int? SequenceNumber { get; set; }

        [JsonPropertyName("userIdAdd")]
        public string UserIdAdd { get; set; }
    }

    public class UpdateTaskRequestHandler : IRequestHandler<UpdateTaskRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<UpdateTaskRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public UpdateTaskRequestHandler(ILogger<UpdateTaskRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(UpdateTaskRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateTaskRequest.Handle - In Process");

            var response = await _db2PolicyService.UpdateTaskAsync(request, cancellationToken);

            _logger.LogInformation("UpdateTaskRequest.Handle - Completed");
            return response;
        }
    }
}
