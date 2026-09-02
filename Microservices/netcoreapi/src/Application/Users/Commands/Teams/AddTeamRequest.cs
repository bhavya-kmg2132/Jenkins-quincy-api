using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.Teams.AddTeam
{
    public class AddTeamRequest : IRequest<string>
    {
        public string TeamName { get; set; }
        public string TeamShortName { get; set; }
        public string Description { get; set; }
        public string TeamOwnerId { get; set; }
        public string TeamCaptainId { get; set; }
    }

    public class AddTeamRequestHandler : IRequestHandler<AddTeamRequest, string>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public AddTeamRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
            this._currentUserService = currentUserService;
        }

        public async Task<string> Handle(AddTeamRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddTeamRequest.Handle - In process");

            var team = new NetAuth.Contract.DataContract.Dto.TeamDto
            {
                TeamName = request.TeamName,
                TeamShortName = request.TeamShortName,
                Description = request.Description,
                TeamOwnerId = request.TeamOwnerId,
                TeamCaptainId = request.TeamCaptainId
            };

            string newId = await _dataAccess.AddTeam(team, _currentUserService.UserId);

            _logger.LogInformation("AddTeamRequest.Handle - Completed");
            return newId;
        }
    }
}
