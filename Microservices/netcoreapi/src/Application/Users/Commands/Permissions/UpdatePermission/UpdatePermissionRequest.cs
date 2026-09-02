using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetAuth.Contract.DataContract.Requests;

namespace Application.Users.Commands.UserPermissions.UpdatePermissionRequest
{
    /// <summary>
    /// class UpdatePermissionRequest extends the IRequest interface of MediatR
    /// </summary>
    public class UpdatePermissionRequest : IRequest<Unit>
    {
        public string Id { get; set; }
        public string PermissionValue { get; set; }
        public string PermissionDisplayName { get; set; }
        public string PermissionSetId { get; set; }
        public string PermissionType { get; set; }
        public string ModuleId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } 
        public bool IsApproved { get; set; }
        public string ApproverId { get; set; }
        public DateTime? ApprovedDateTime { get; set; }
        public bool? IsAuthorized { get; set; }
        public string AuthorizedById { get; set; }
        public DateTime? AuthorizedDateTime { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request , created UpdatePermissionRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class UpdatePermissionRequestHandler : IRequestHandler<UpdatePermissionRequest, Unit>
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
        public UpdatePermissionRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess userDataAccess, ICurrentUserService currentUserService, IIdentityManager identityManager)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._userDataAccess = userDataAccess;
            this._mapper = mapper;
            this._currentUserService = currentUserService;
            this._identityManager = identityManager;
        }


        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Unit</returns>
        public async Task<Unit> Handle(UpdatePermissionRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information : In Process
            _logger.LogInformation("UpdatePermissionRequest.Handle - In process");

            //2. Find the permission for update in database
            var permissions = await _identityManager.GetAllPermissionsAsync();
            var originalEntity = permissions.FirstOrDefault(x => x.PermissionId == request.Id);

            //3. if the entity not found then throw NotFoundException
            if (originalEntity == null)
            {
                throw new NotFoundException(nameof(NetAuth.Contract.DataContract.Requests.UpdatePermission), request.Id);
            }

            //4. Deep copy existing object before overriding it with new values
            ///var newEntity = (UpdatePermission)Helper.CloneObject(originalEntity);
           

            //4. Build update request from the incoming request values
            var newEntity = new NetAuth.Contract.DataContract.Requests.UpdatePermission();

            //5. Update entities with new values recieved in request object.
            newEntity.Id = request.Id;
            newEntity.PermissionDisplayName = request.PermissionDisplayName;
            newEntity.PermissionSetId = request.PermissionSetId;
            newEntity.PermissionType = request.PermissionType;
            if (!(request.PermissionType.ToUpper() == "ACTION"))
            {
                newEntity.PermissionValue = request.PermissionValue;
            }
            newEntity.PermissionValue = request.PermissionValue;
            newEntity.ModuleId = request.ModuleId;
            newEntity.IsActive = request.IsActive;
            //newEntity.IsDeleted = request.IsDeleted;
            newEntity.IsApproved = request.IsApproved;
            newEntity.ApproverId = request.ApproverId;
            newEntity.ApprovedDateTime = request.ApprovedDateTime;
            newEntity.IsAuthorized = request.IsAuthorized;
            newEntity.AuthorizedById = request.AuthorizedById;
            newEntity.AuthorizedDateTime = request.AuthorizedDateTime;

            //6. Update permissions  for user
            await _userDataAccess.UpdatePermission(newEntity, _currentUserService.UserName);

            //7. Logging Information : Completed
            _logger.LogInformation("UpdatePermissionRequest.Handle - Completed");

            await Task.CompletedTask;

            //8. Return Unit
            return Unit.Value;
        }
    }
}


