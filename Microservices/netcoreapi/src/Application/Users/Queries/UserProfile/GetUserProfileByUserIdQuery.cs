using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.UserProfile.GetUserProfileByUserIdQuery
{
    /// <summary>
    /// class GetUserProfileByUserIdQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetUserProfileByUserIdQuery : IRequest<UserProfileListVm>
    {
        public string UserId { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created GetUserProfileByUserIdQuery class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetPermissionsQueryHandler : IRequestHandler<GetUserProfileByUserIdQuery, UserProfileListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates of GetUserProfileByUserIdQuery class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetPermissionsQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._mapper = mapper;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserProfileListVm> Handle(GetUserProfileByUserIdQuery request, CancellationToken cancellationToken)
        {
            return new UserProfileListVm
            {
                //Mapping UserProfileDto with UsersProfile entity
                UserProfileList = _mapper.Map<List<UserProfileDto>>(await _dataAccess.GetUserProfileByUserId(request.UserId))
            };
        }
    }
}

