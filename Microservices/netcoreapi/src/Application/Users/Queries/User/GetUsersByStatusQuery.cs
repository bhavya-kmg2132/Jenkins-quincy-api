using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.GetUsersByStatusQuery
{
    public class GetUsersByStatusQuery : IRequest<List<UsersDto>> // Return type updated to List<UsersDto>
    {
        public string Status { get; set; }
    }

    public class GetUsersByStatusQueryHandler : IRequestHandler<GetUsersByStatusQuery, List<UsersDto>>
    {
        private readonly ILogger<GetUsersByStatusQueryHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _userDataAccess;
        private readonly IMapper _mapper;

        public GetUsersByStatusQueryHandler(
        IConfiguration configuration,
        ILogger<GetUsersByStatusQueryHandler> logger,
        IMapper mapper,
        IUserDataAccess userDataAccess)
        {
            _configuration = configuration;
            _userDataAccess = userDataAccess;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<UsersDto>> Handle(GetUsersByStatusQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetUsersByStatusQuery.Handle - In process");

            var allUsers = _mapper.Map<List<UsersDto>>(await _userDataAccess.GetUsersByStatus(request.Status));

            _logger.LogInformation("GetUsersByStatusQuery.Handle - Completed");

            return allUsers;
        }
    }
}