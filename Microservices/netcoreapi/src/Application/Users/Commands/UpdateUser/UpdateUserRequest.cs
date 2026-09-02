using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.UpdateUser
{
    public class UpdateUserRequest : IRequest<int>
    {
        public string userId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string EmpId { get; set; }
        public string oid { get; set; }
        public string userName { get; set; }

    }

    /// <summary>
    /// For Creating handler for the above request , created UpdateUserRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class UpdateUserRequestHandler : IRequestHandler<UpdateUserRequest, int>
    {
        private readonly ILogger _logger;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityManager _identityManager;



        /// <summary>
        /// Instantiates the class UpdateUserRequestHandler
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public UpdateUserRequestHandler(ILogger logger, IUserDataAccess userDataAccess, ICurrentUserService currentUserService, IIdentityManager identityManager)
        {
            this._logger = logger;
            this._userDataAccess = userDataAccess;
            this._currentUserService = currentUserService;
            this._identityManager = identityManager;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>UserId of created user</returns>
        public async Task<int> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("UpdateUserRequest.Handle - In process");

            //2. Assign values to user Entity 
            NetAuth.Contract.DataContract.Requests.UpdateUser updateUser = new NetAuth.Contract.DataContract.Requests.UpdateUser();
            updateUser.userId = request.userId;
            updateUser.Email = request.Email;
            updateUser.PhoneNumber = request.PhoneNumber;
            updateUser.EmpId = request.EmpId;
            updateUser.UpdatedBy = _currentUserService.UserId;
            updateUser.UpdatedDateTime = DateTime.UtcNow;

            //3. Call the UpdateUser Method in DataAccess layer
            int updateStatus = await _userDataAccess.UpdateUser(updateUser);

            //3.1 Invalidate this user's cached identity. GetIdentityUserAsync gets called with whichever
            //    identifier the caller has on hand (userId, oid, or userName), so all three must be
            //    invalidated to guarantee a stale cache entry isn't served under any of them.
            if (!string.IsNullOrEmpty(request.userName))
                await _identityManager.ResetIdentityUserCache(request.userName);
            if (!string.IsNullOrEmpty(request.userId))
                await _identityManager.ResetIdentityUserCache(request.userId);
            if (!string.IsNullOrEmpty(request.oid))
                await _identityManager.ResetIdentityUserCache(request.oid);


            //4. Logging Information : Completed
            _logger.LogInformation("UpdateUserRequest.Handle - Completed");

            //5. Return id
            return updateStatus;
        }
    }
}
