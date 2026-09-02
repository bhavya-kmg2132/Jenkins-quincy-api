using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.UpdateUserPasswordHash
{
    /// <summary>
    /// class UpdateUserPasswordHashRequest extends the IRequest interface of MediatR
    /// </summary>
    public class UpdateUserPasswordHashRequest : IRequest<int>
    {
        public string UserId { get; set; }
        public string PasswordHash { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string UpdateReason { get; set; }


    }

    /// <summary>
    /// For Creating handler for the above request , created UpdateUserPasswordHashRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class UpdateUserPasswordHashRequestHandler : IRequestHandler<UpdateUserPasswordHashRequest, int>
    {
        private readonly ILogger _logger;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ICurrentUserService _currentUserService;


        /// <summary>
        /// Instantiates the class UpdateUserPasswordHashRequestHandler
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public UpdateUserPasswordHashRequestHandler(ILogger logger, IUserDataAccess userDataAccess, ICurrentUserService currentUserService)
        {
            this._logger = logger;
            this._userDataAccess = userDataAccess;
            this._currentUserService = currentUserService;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>UserId of created user</returns>
        public async Task<int> Handle(UpdateUserPasswordHashRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("UpdateUserPasswordHashRequest.Handle - In process");

            var existingCredential = await _userDataAccess.GetUserPasswordHash(request.UserId);

            if (!string.IsNullOrEmpty(request.OldPassword))
            {
                bool isValid = BCrypt.Net.BCrypt.Verify(
                    request.OldPassword,
                    existingCredential.PasswordHash
                );

                if (!isValid)
                    throw new ApplicationException("Old password is incorrect.");

                bool samePassword = BCrypt.Net.BCrypt.Verify(
                    request.NewPassword,
                    existingCredential.PasswordHash
                );

                if (samePassword)
                    throw new ApplicationException("New password cannot be same as old password.");
            }
            else
            {
                // Admin flow
                if (!_currentUserService.UserRoles.Contains("Admin"))
                    throw new UnauthorizedAccessException("Only admin can reset password.");
            }

            var newHashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            //2. Assign values to UserPasswordHash Entity 
            var entity = new NetAuth.Contract.DataContract.Requests.UpdateUserPasswordHash
            {
                UserId = request.UserId,
                PasswordHash = newHashedPassword,
                UpdatedDateTime = DateTime.UtcNow,
                UpdatedBy = _currentUserService.UserId,
                UpdateReason = request.UpdateReason
            };

            //3. Call the UpdateUserPasswordHash Method in DataAccess layer
            int updateStatus = await _userDataAccess.UpdateUserPasswordHash(entity);

            //4. Logging Information : Completed
            _logger.LogInformation("UpdateUserPasswordHashRequest.Handle - Completed");

            //5. Return id
            return updateStatus;
        }
    }
}



