using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.UserActivity.GetUserActivityQuery
{
    /// <summary>
    /// class GetUserActivitiesQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetUserActivitiesQuery : IRequest<UserActivitiesVm>
    {
        public string UserId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int period { get; set; } = 90;
    }

    /// <summary>
    /// For Creating handler for the above request, created GetUserActivitiesQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetUserActivitiesQueryHandler : IRequestHandler<GetUserActivitiesQuery, UserActivitiesVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetUserActivitiesQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetUserActivitiesQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess)
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
        public async Task<UserActivitiesVm> Handle(GetUserActivitiesQuery request, CancellationToken cancellationToken)
        {
            var userActivities = new UserActivitiesVm
            {
                //Mapping UsersDto with Usersentity
                UserActivities = _mapper.Map<List<UserActivityDto>>(await _dataAccess.GetUserActivities(request.UserId, request.PageSize, request.PageNumber, request.period))
            };

            var userName = await _dataAccess.GetUserFullName();
            foreach (var activity in userActivities.UserActivities)
            {
                activity.Name = userName.FirstOrDefault(s => s.Id == activity.Id)?.FullName ?? "Unknown User";
            }
            return userActivities;
        }
    }
}

