using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Notepad.Commands.CreateNotepad
{
    public class CreateNotepadRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("transactionType")]
        public string TransactionType { get; set; }

        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }
    }

    public class CreateNotepadRequestHandler : IRequestHandler<CreateNotepadRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<CreateNotepadRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public CreateNotepadRequestHandler(ILogger<CreateNotepadRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(CreateNotepadRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateNotepadRequest.Handle - In Process");

            var response = await _db2PolicyService.CreateNotepadAsync(request, cancellationToken);

            _logger.LogInformation("CreateNotepadRequest.Handle - Completed");
            return response;
        }
    }
}
