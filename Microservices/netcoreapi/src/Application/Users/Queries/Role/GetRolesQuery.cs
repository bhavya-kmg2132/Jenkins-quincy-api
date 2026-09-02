using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Role.Queries.GetRole;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.Role.GetRoleQuery
{
    /// <summary>
    /// class GetRolesQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetRolesQuery : IRequest<RoleListVm>
    {

    }

    /// <summary>
    /// For Creating handler for the above request, created GetRolesQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, RoleListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates of GetRolesQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetRolesQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess)
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
        public async Task<RoleListVm> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            //Returns RoleListVm
            return new RoleListVm
            {
                //Mapping RoleDto with Role entity
                RolesList = _mapper.Map<List<RoleDto>>(await _dataAccess.GetRoles())
            };
        }
    }
}

