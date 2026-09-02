using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExternalPolicy.Notepad.Queries.GetNotepadDetail
{
    public class GetNotepadDetailQuery : IRequest<ExternalPolicyResponse>
    {
        [JsonPropertyName("notepadId")]
        public string NotepadId { get; set; }
    }

    public class GetNotepadDetailQueryHandler : IRequestHandler<GetNotepadDetailQuery, ExternalPolicyResponse>
    {
        private readonly ILogger<GetNotepadDetailQueryHandler> _logger;
        private readonly IDb2PolicyService _db2PolicyService;

        public GetNotepadDetailQueryHandler(ILogger<GetNotepadDetailQueryHandler> logger, IDb2PolicyService db2PolicyService)
        {
            _logger = logger;
            _db2PolicyService = db2PolicyService;
        }

        public async Task<ExternalPolicyResponse> Handle(GetNotepadDetailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetNotepadDetailQuery.Handle - In Process");

            var response = await _db2PolicyService.GetNotepadDetailAsync(request, cancellationToken);

            _logger.LogInformation("GetNotepadDetailQuery.Handle - Completed");
            return response;
        }
    }
}
