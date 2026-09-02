using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.TaskManager.Queries.GetTasks
{
    public class GetTasksQuery : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("taskCode")]
        public string TaskCode { get; set; }

        [JsonPropertyName("taskStatus")]
        public string TaskStatus { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("transactionDate")]
        public string TransactionDate { get; set; }

        [JsonPropertyName("paymentDueDate")]
        public string PaymentDueDate { get; set; }

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
    }

    public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetTasksQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public GetTasksQueryHandler(ILogger<GetTasksQueryHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetTasksQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTasksQuery.Handle - In Process");

            var response = await _db2PolicyService.GetTasksAsync(request, cancellationToken);

            _logger.LogInformation("GetTasksQuery.Handle - Completed");
            return response;
        }
    }
}
