using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.AddRoles
{
    /// <summary>
    /// class AddRolesForUserRequest extends the IRequest interface of MediatR
    /// </summary>
    public class AddRolesForUserRequest : IRequest<Unit>
    {
        public List<string> RoleIds { get; set; }
        public string UserId { get; set; }

    }

    /// <summary>
    /// For Creating handler for the above request , created AddRolesForUserRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class AddRolesForUserRequestHandler : IRequestHandler<AddRolesForUserRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates the class AddRolesForUserRequestHandler
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public AddRolesForUserRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess userDataAccess, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._userDataAccess = userDataAccess;
            this._mapper = mapper;
            this._currentUserService = currentUserService;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>UserId of created prospect</returns>
        public async Task<Unit> Handle(AddRolesForUserRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("AddRolesForUserRequest.Handle - In process");

            //2. Assign requested Roles values for user
            string createdBy = _currentUserService.oid;
            await _userDataAccess.AddRolesForUser(request.UserId, request.RoleIds, createdBy);

            //3. Logging Information : Completed
            _logger.LogInformation("AddRolesForUserRequest.Handle - Completed");

            //4. Return Unit
            return Unit.Value;
        }
    }
}


