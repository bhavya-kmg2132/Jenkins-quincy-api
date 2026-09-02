using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.User.Command.UpdateUserAccessLevel
{
    /// <summary>
    /// class UpdateUserAccessLevelRequest extends the IRequest interface of MediatR
    /// </summary>
    /// 
    //[InvalidateCache(typeof(GetProspectQuery))]
    public class UpdateUserAccessLevelRequest : IRequest<Unit>
    {
        public string UserId { get; set; }
        public string AccessLevelValue { get; set; }
        public string UserName { get; set; }

    }

    /// <summary>
    /// For Creating handler for the above request , created UpdateUserAccessLevelRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class UpdateUserAccessLevelRequestHandler : IRequestHandler<UpdateUserAccessLevelRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityManager _identityManager;


        /// <summary>
        ///  Instantiates UpdateUserAccessLevelRequestHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public UpdateUserAccessLevelRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess userDataAccess, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._userDataAccess = userDataAccess;
            this._currentUserService = currentUserService;
            this._mapper = mapper;
            this._identityManager = _identityManager;

        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Unit</returns>
        public async Task<Unit> Handle(UpdateUserAccessLevelRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information In Process
            _logger.LogInformation("UpdateUserAccessLevelRequest.Handle - In process");

            //2. CreatedBy property from currentUserService
            string CreatedUser = _currentUserService.UserName;

            //3. Find the requested Id for update in database
            var entity = await _userDataAccess.GetUserFromDbAsync(request.UserId);

            //4. If the Id does'nt exist, throw NotFoundException
            if (entity == null)
            {
                throw new NotFoundException(nameof(User), request.UserId);
            }

            //5. Assigning requested update values for prospect, prospect information and contact
            entity.Id = request.UserId;
            entity.AccessLevel = request.AccessLevelValue;
            entity.UpdatedBy = _currentUserService.UserName;

            //6. Updating the Company Info, Policy Info, Risk Info and Contact Info
            await _userDataAccess.UpdateUserAccessLevel(entity);

            //6.1 Invalidate this user's cached identity so the new access level is reflected without
            //    waiting for the cache's normal expiry.
            await _identityManager.ResetIdentityUserCache(request.UserName);

            //7. Logging Information : Completed
            _logger.LogInformation("UpdateUserAccessLevelRequest.Handle - Completed");

            //8. Return unit
            return Unit.Value;
        }
    }
}
