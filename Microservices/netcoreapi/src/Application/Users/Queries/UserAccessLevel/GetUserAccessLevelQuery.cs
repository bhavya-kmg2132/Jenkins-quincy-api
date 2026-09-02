using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.AccessLevel
{
    /// <summary>
    /// class GetUserAccessLevelQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetUserAccessLevelQuery : IRequest<UserAccessLevelListVm>
    {

    }

    /// <summary>
    /// For Creating handler for the above request , created GetAccessLevelQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetAccessLevelQueryHandler : IRequestHandler<GetUserAccessLevelQuery, UserAccessLevelListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _userDataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates of GetAccessLevelQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetAccessLevelQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess userDataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._userDataAccess = userDataAccess;
            this._mapper = mapper;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserAccessLevelListVm> Handle(GetUserAccessLevelQuery request, CancellationToken cancellationToken)
        {
            //Returns UserAccessLevelListVm
            return new UserAccessLevelListVm
            {
                //Mapping UserAccessLevelDto with UserAccessLevel Entity
                UserAccessLevelList = _mapper.Map<List<UserAccessLevelDto>>(await _userDataAccess.GetUserAccessLevelList())
            };
        }
    }
}

