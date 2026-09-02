using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Users.Commands.UserPermissions.UpdatePermissionRequest;

namespace Application.Users.Commands.Permissions
{
    public class UpdatePermissionValidatorRequest : AbstractValidator<UpdatePermissionRequest>
    {
        private readonly IIdentityManager _dataAccess;
        /// <summary>
        /// Validates Update Permission request
        /// </summary>
        public UpdatePermissionValidatorRequest(IIdentityManager dataAccess)
        {
            this._dataAccess = dataAccess;
            // 1) Id
            // It is mandatory.
            RuleFor(x => x.Id)
               .NotEmpty()
               .WithMessage(Resources.ErrorMessages.UpdatePermissionRequestValidator_PermissionId_NotEmpty);

            //2) PermissionValue
            //It is mandatory.
            RuleFor(x => x.PermissionValue)
              .NotEmpty()
              .WithMessage(Resources.ErrorMessages.UpdatePermissionRequestValidator_PermissionValue_NotEmpty)

              //2.1 PermissionValue shouldn't be duplicate
              .MustAsync(IsPermissionValueDuplicate)
              .WithMessage(Resources.ErrorMessages.UpdatePermissionRequestValidator_Permission_MustUnique)

            //2.2 PermissionValue must not update. if PermissionType is ACTION
              .MustAsync(IsPermissionTypeActionAndPermissionValueChanged)
              .WithMessage(Resources.ErrorMessages.UpdatePermissionRequestValidator_PermissionType_Must_Not_Be_Action);

            //3) PermissionDisplayName
            //It is mandatory.
            RuleFor(x => x.PermissionDisplayName)
               .NotEmpty()
               .WithMessage(Resources.ErrorMessages.UpdatePermissionRequestValidator_PermissionDisplayName_NotEmpty);

            //4) PermissionSetId
            //It is mandatory.
            RuleFor(x => x.PermissionSetId)
               .NotEmpty()
               .WithMessage(Resources.ErrorMessages.UpdatePermissionRequestValidator_PermissionSetId_NotEmpty);

            //5) ModuleId
            //It is mandatory.
            RuleFor(x => x.ModuleId)
               .NotEmpty()
               .WithMessage(Resources.ErrorMessages.UpdatePermissionRequestValidator_ModuleId_NotEmpty);

            //5) PermissionType
            //It cannot be changed once set.
            RuleFor(x => x.PermissionType)
               .MustAsync(IsPermissionTypeChangeAllowed)
              .WithMessage(Resources.ErrorMessages.UpdatePermissionRequestValidator_PermissionType_Change_Not_Allowed);


        }

        /// <summary>
        /// IsPermissionValueDuplicate
        /// </summary>
        /// <param name="request"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>bool</returns>
        public async Task<bool> IsPermissionValueDuplicate(UpdatePermissionRequest request, string name, CancellationToken cancellationToken)
        {
            // get permissions
            var permissions = await _dataAccess.GetAllPermissionsAsync();

            // Check for permission if already exists, and not for this id
            if (permissions.Exists(x => x.PermissionValue.Equals(request.PermissionValue)
                                                   && x.PermissionDisplayName.Equals(request.PermissionDisplayName)
                                                   && !x.PermissionId.Equals(request.Id)))
            {
                // if permission already exist then return false
                return false;
            }
            return true;
        }

        public async Task<bool> IsPermissionTypeActionAndPermissionValueChanged(UpdatePermissionRequest request, string name, CancellationToken cancellationToken)
        {
            var permissions = await _dataAccess.GetAllPermissionsAsync();

            var existingPermission = permissions
                .FirstOrDefault(x => x.PermissionId == request.Id);

            if (existingPermission == null)
            {
                return true;
            }

            bool isActionPermission =
                string.Equals(existingPermission.PermissionType, "ACTION",
                    StringComparison.OrdinalIgnoreCase);

            bool permissionValueChanged =
                !string.Equals(existingPermission.PermissionValue,
                    request.PermissionValue,
                    StringComparison.Ordinal);

            if (isActionPermission && permissionValueChanged)
            {
                // Fire validation
                return false;
            }

            return true;
        }

        public async Task<bool> IsPermissionTypeChangeAllowed(UpdatePermissionRequest request, string name, CancellationToken cancellationToken)
        {
            var permissions = await _dataAccess.GetAllPermissionsAsync();

            var existingPermission = permissions
                .FirstOrDefault(x => x.PermissionId == request.Id);

            if (existingPermission == null)
            {
                return true;
            }

            bool permissionTypeChanged =
                !string.Equals(existingPermission.PermissionType,
                    request.PermissionType,
                    StringComparison.OrdinalIgnoreCase);

            return !permissionTypeChanged;
        }

    }
}


