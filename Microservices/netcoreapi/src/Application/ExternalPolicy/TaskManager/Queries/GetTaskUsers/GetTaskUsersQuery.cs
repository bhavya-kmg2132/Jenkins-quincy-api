using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.TaskManager.Queries.GetTaskUsers
{
    public class GetTaskUsersQuery : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("taskRole")]
        public string TaskRole { get; set; }

        [JsonPropertyName("companyNumber")]
        public string CompanyNumber { get; set; }

        [JsonPropertyName("businessLine")]
        public string BusinessLine { get; set; }
    }

    public class GetTaskUsersQueryHandler : IRequestHandler<GetTaskUsersQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetTaskUsersQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public GetTaskUsersQueryHandler(ILogger<GetTaskUsersQueryHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetTaskUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTaskUsersQuery.Handle - In Process");

            var response = await _db2PolicyService.GetTaskUsersAsync(request, cancellationToken);

            _logger.LogInformation("GetTaskUsersQuery.Handle - Completed");
            return response;
        }
    }
}
