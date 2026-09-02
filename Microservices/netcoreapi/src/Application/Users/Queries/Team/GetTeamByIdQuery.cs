using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Users.Queries.Team;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.Team.GetTeamByIdQuery
{
    public class GetTeamByIdQuery : IRequest<TeamVm>
    {
        public string TeamId { get; set; }
    }

    public class GetTeamByIdQueryHandler : IRequestHandler<GetTeamByIdQuery, TeamVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;

        public GetTeamByIdQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
        }

        public async Task<TeamVm> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTeamByIdQuery.Handle - In process");

            return _mapper.Map<TeamVm>(await _dataAccess.GetTeamById(request.TeamId));
        }
    }
}
