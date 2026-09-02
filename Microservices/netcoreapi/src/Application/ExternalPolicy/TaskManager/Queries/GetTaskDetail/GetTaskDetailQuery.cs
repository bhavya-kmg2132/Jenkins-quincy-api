using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.TaskManager.Queries.GetTaskDetail
{
    public class GetTaskDetailQuery : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("taskCode")]
        public string TaskCode { get; set; }

        [JsonPropertyName("winsTransactionCode")]
        public string WinsTransactionCode { get; set; }

        [JsonPropertyName("sequenceNumber")]
        public int? SequenceNumber { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }

    public class GetTaskDetailQueryHandler : IRequestHandler<GetTaskDetailQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetTaskDetailQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public GetTaskDetailQueryHandler(ILogger<GetTaskDetailQueryHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetTaskDetailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTaskDetailQuery.Handle - In Process");

            var response = await _db2PolicyService.GetTaskDetailAsync(request, cancellationToken);

            _logger.LogInformation("GetTaskDetailQuery.Handle - Completed");
            return response;
        }
    }
}
