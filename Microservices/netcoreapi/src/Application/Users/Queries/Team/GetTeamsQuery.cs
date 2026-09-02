using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Users.Queries.Team;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.Team.GetTeamsQuery
{
    public class GetTeamsQuery : IRequest<TeamListVm>
    {
    }

    public class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, TeamListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;

        public GetTeamsQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
        }

        public async Task<TeamListVm> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTeamsQuery.Handle - In process");

            return new TeamListVm
            {
                Teams = _mapper.Map<List<TeamVm>>(await _dataAccess.GetTeams())
            };
        }
    }
}
