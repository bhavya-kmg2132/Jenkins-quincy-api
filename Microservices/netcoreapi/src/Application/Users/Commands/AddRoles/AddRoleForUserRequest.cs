using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.AddRoles
{
    /// <summary>
    /// class AddRoleForUserRequest extends the IRequest interface of MediatR
    /// </summary>
    public class AddRoleForUserRequest : IRequest<Unit>
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string UserName { get; set; }

    }

    /// <summary>
    /// For Creating handler for the above request , created AddRoleForUserRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class AddRoleForUserRequestHandler : IRequestHandler<AddRoleForUserRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IIdentityManager _identityManager;


        /// <summary>
        /// Instantiates the class CreateProspectRequestHandler
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public AddRoleForUserRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess userDataAccess, ICurrentUserService currentUserService, IIdentityManager identityManager)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._userDataAccess = userDataAccess;
            this._mapper = mapper;
            this._currentUserService = currentUserService;
            this._identityManager = identityManager;
            _identityManager = identityManager;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Unit</returns>
        public async Task<Unit> Handle(AddRoleForUserRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("AddRoleForUserRequest.Handle - In process");

            string createdBy = _currentUserService.oid;

            //2. Assign requested Role values for user
            await _userDataAccess.AddRoleForUser(request.UserId, request.RoleId, createdBy);

            //2.1 Invalidate this user's cached identity so the new role is reflected immediately.
            await _identityManager.ResetIdentityUserCache(request.UserName);


            //5. Logging Information : Completed
            _logger.LogInformation("AddRoleForUserRequest.Handle - Completed");

            //6. Return Unit
            return Unit.Value;
        }
    }
}


