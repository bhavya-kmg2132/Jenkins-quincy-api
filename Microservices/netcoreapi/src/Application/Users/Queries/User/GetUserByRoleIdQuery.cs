using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries
{
    /// <summary>
    /// class GetUserByRoleIdQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetUserByRoleIdQuery : IRequest<UserListVm>
    {
        public string RoleId { get; set; }

    }

    /// <summary>
    /// For Creating handler for the above request , created GetUserByRoleIdQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetUserByRoleIdQueryHandler : IRequestHandler<GetUserByRoleIdQuery, UserListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IIdentityManager _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetUserByRoleIdQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetUserByRoleIdQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IIdentityManager dataAccess)
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
        public async Task<UserListVm> Handle(GetUserByRoleIdQuery request, CancellationToken cancellationToken)
        {
            //Returns User
            return new UserListVm
            {
                UserList = _mapper.Map<List<UserDto>>(await _dataAccess.GetUserByRoleIdAsync(request.RoleId))
            };
        }
    }
}


