using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.Teams.AddTeamMembers
{
    public class AddTeamMembersRequest : IRequest<Unit>
    {
        public string TeamId { get; set; }
        public List<string> UserIds { get; set; }
    }

    public class AddTeamMembersRequestHandler : IRequestHandler<AddTeamMembersRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public AddTeamMembersRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
            this._currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(AddTeamMembersRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddTeamMembersRequest.Handle - In process");

            await _dataAccess.AddTeamMembers(request.TeamId, request.UserIds, _currentUserService.UserId);

            _logger.LogInformation("AddTeamMembersRequest.Handle - Completed");
            return Unit.Value;
        }
    }
}
