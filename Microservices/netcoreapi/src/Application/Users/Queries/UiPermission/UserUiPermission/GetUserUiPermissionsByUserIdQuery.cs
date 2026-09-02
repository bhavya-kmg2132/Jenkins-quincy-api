using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Query.UiPermission.UserUiPermission;
using Application.Users.Queries.UiPermission.UserUiPermission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.UserUiPermision.GetUserUiPermissionsByUserIdQuery
{
    /// <summary>
    /// class GetUserUiPermissionsByUserIdQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetUserUiPermissionsByUserIdQuery : IRequest<UserUiPermissionListVm>
    {
        public string UserId { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created GetUserUiPermissionsByUserIdQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetUserUiPermissionsByUserIdQueryHandler : IRequestHandler<GetUserUiPermissionsByUserIdQuery, UserUiPermissionListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates of GetUserUiPermissionsByUserIdQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetUserUiPermissionsByUserIdQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess)
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
        /// <returns>UserUiPermissionListVm</returns>
        public async Task<UserUiPermissionListVm> Handle(GetUserUiPermissionsByUserIdQuery request, CancellationToken cancellationToken)
        {
            //Returns UserUiPermissionListVm
            return new UserUiPermissionListVm
            {
                //Mapping UserUiPermissionDto with UserUiPermission
                UserUiPermissions = _mapper.Map<List<UserUiPermissionDto>>(await _dataAccess.GetUserUiPermissionsByUserId(request.UserId))
            };
        }
    }
}

