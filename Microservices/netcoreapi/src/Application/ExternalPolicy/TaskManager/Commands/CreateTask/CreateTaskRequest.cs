using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.TaskManager.Commands.CreateTask
{
    public class CreateTaskRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("userIdAdd")]
        public string UserIdAdd { get; set; }

        [JsonPropertyName("taskCode")]
        public string TaskCode { get; set; }

        [JsonPropertyName("taskDescription")]
        public string TaskDescription { get; set; }

        [JsonPropertyName("assignedTo")]
        public string AssignedTo { get; set; }

        [JsonPropertyName("paymentDueDate")]
        public string PaymentDueDate { get; set; }
    }

    public class CreateTaskRequestHandler : IRequestHandler<CreateTaskRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<CreateTaskRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public CreateTaskRequestHandler(ILogger<CreateTaskRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(CreateTaskRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateTaskRequest.Handle - In Process");

            var response = await _db2PolicyService.CreateTaskAsync(request, cancellationToken);

            _logger.LogInformation("CreateTaskRequest.Handle - Completed");
            return response;
        }
    }
}
