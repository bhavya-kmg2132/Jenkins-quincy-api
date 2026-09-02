using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.UserActivity.GetUserActivitiesByUserIdsQuery
{
    /// <summary>
    /// class GetUserActivitiesByUserIdsQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetUserActivitiesByUserIdsQuery : IRequest<UserActivitiesVm>
    {
        public List<string> UserIds { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created GetUserActivitiesByUserIdsQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetUserActivitiesByUserIdsQueryHandler : IRequestHandler<GetUserActivitiesByUserIdsQuery, UserActivitiesVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetUserActivitiesByUserIdsQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetUserActivitiesByUserIdsQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
        }

        /// <summary>
        /// Handler will recieve request, process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>UserActivitiesVm</returns>
        public async Task<UserActivitiesVm> Handle(GetUserActivitiesByUserIdsQuery request, CancellationToken cancellationToken)
        {

            //1. Returns UserActivities
            return new UserActivitiesVm
            {
                //Mapping UsersDto with Usersentity
                UserActivities = _mapper.Map<List<UserActivityDto>>(await _dataAccess.GetUserActivitiesByUserIds(request.UserIds))
            };
        }
    }
}

