using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.Team.GetTeamMembersByTeamIdQuery
{
    public class GetTeamMembersByTeamIdQuery : IRequest<TeamMemberListVm>
    {
        public string TeamId { get; set; }
    }

    public class GetTeamMembersByTeamIdQueryHandler : IRequestHandler<GetTeamMembersByTeamIdQuery, TeamMemberListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;

        public GetTeamMembersByTeamIdQueryHandler(IConfiguration configuration, ILogger logger, IUserDataAccess dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
        }

        public async Task<TeamMemberListVm> Handle(GetTeamMembersByTeamIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTeamMembersByTeamIdQuery.Handle - In process");

            var members = await _dataAccess.GetTeamMembersByTeamId(request.TeamId);

            _logger.LogInformation("GetTeamMembersByTeamIdQuery.Handle - Completed");

            return new TeamMemberListVm
            {
                TeamMembers = members?.Select(m => new TeamMemberVm
                {
                    MemberId = m.MemberId,
                    Email = m.Email,
                    UserName = m.UserName,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Mobile = m.Mobile,
                    preferred_username = m.preferred_username,
                    Designation = m.Designation,
                    Department = m.Department,
                    AccessLevel = m.AccessLevel
                }).ToList()
            };
        }
    }
}
