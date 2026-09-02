using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Users.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.AllUsers.Queries.AllUser.GetUsersQueryAsync
{
    /// <summary>
    /// class GetAllUserQueryHandler extends the IRequest interface of MediatR
    /// </summary>
    public class GetUsersQueryAsync : IRequest<UserListVm>
    {

    }

    /// <summary>
    /// For Creating handler for the above request , created GetAllUserQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetUsersQueryAsyncHandler : IRequestHandler<GetUsersQueryAsync, UserListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IIdentityManager _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetAllUsersQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetUsersQueryAsyncHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IIdentityManager dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            _mapper = mapper;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserListVm> Handle(GetUsersQueryAsync request, CancellationToken cancellationToken)
        {
            //1. Logging Information - In process
            _logger.LogInformation("GetUsersQueryAsync - In process");

            //2. Returns AllUserslist
            return new UserListVm
            {
                //2.1 Mapping AllUsersDto with AllUsersentity
                UserList = _mapper.Map<List<UserDto>>(await _dataAccess.GetUsersAsync())
            };
        }
    }
}

