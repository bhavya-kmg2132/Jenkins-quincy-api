using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.Teams.RemoveTeamMember
{
    public class RemoveTeamMemberRequest : IRequest<Unit>
    {
        public string TeamId { get; set; }
        public string MemberId { get; set; }
    }

    public class RemoveTeamMemberRequestHandler : IRequestHandler<RemoveTeamMemberRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;

        public RemoveTeamMemberRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
        }

        public async Task<Unit> Handle(RemoveTeamMemberRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("RemoveTeamMemberRequest.Handle - In process");

            await _dataAccess.RemoveTeamMember(request.TeamId, request.MemberId);

            _logger.LogInformation("RemoveTeamMemberRequest.Handle - Completed");
            return Unit.Value;
        }
    }
}
