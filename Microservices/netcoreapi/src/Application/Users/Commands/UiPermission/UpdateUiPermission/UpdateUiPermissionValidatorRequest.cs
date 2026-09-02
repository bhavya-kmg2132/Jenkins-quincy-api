using Application.Common.Interfaces;
using Application.Users.Commands.UiPermission.UpdateUiPermission;

namespace Application.Users.Commands.UiPermission
{
    public class UpdateUiPermissionValidatorRequest : AbstractValidator<UpdateUiPermissionRequest>
    {
        private readonly IUiPermissionDataAccess _dataAccess;
        /// <summary>
        /// Validates Update UiPermission request
        /// </summary>
        public UpdateUiPermissionValidatorRequest(IUiPermissionDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;
            // 1) Id
            // It is mandatory.
            RuleFor(x => x.PermissionId)
               .NotEmpty()
               .WithMessage(Resources.ErrorMessages.UpdateUiPermissionRequestValidator_PermissionId_NotEmpty);

            //2) PermissionDisplayName
            //It is mandatory.
            RuleFor(x => x.PermissionDisplayName)
               .NotEmpty()
               .WithMessage(Resources.ErrorMessages.UpdateUiPermissionRequestValidator_PermissionDisplayName_NotEmpty);

            //3) IsActive
            //It is mandatory.
            RuleFor(x => x.IsActive)
               .NotNull()
               .WithMessage(Resources.ErrorMessages.UpdateUiPermissionRequestValidator_IsActive_NotNull);
        }
    }
}


