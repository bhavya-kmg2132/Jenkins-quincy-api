using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Notepad.Commands.UpdateNotepad
{
    public class UpdateNotepadRequest : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("notepadId")]
        public string NotepadId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("updatedBy")]
        public string UpdatedBy { get; set; }
    }

    public class UpdateNotepadRequestHandler : IRequestHandler<UpdateNotepadRequest, ExternalPolicyResponse>
    {
        private readonly ILogger<UpdateNotepadRequestHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public UpdateNotepadRequestHandler(ILogger<UpdateNotepadRequestHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(UpdateNotepadRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateNotepadRequest.Handle - In Process");

            var response = await _db2PolicyService.UpdateNotepadAsync(request, cancellationToken);

            _logger.LogInformation("UpdateNotepadRequest.Handle - Completed");
            return response;
        }
    }
}
