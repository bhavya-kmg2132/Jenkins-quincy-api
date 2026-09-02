using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Users.Queries.Team;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.Team.GetTeamsByUserIdQuery
{
    public class GetTeamsByUserIdQuery : IRequest<TeamListVm>
    {
        public string UserId { get; set; }
    }

    public class GetTeamsByUserIdQueryHandler : IRequestHandler<GetTeamsByUserIdQuery, TeamListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;

        public GetTeamsByUserIdQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
        }

        public async Task<TeamListVm> Handle(GetTeamsByUserIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTeamsByUserIdQuery.Handle - In process");

            return new TeamListVm
            {
                Teams = _mapper.Map<List<TeamVm>>(await _dataAccess.GetTeamsByUserId(request.UserId))
            };
        }
    }
}
