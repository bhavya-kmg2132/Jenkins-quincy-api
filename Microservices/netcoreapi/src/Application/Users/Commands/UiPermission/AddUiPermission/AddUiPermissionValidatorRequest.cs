using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Users.Commands.UiPermission.AddUiPermission;

namespace Application.Users.Commands.UiPermissions
{
    public class AddUiPermissionValidatorRequest : AbstractValidator<AddUiPermissionRequest>
    {
        private readonly IUiPermissionDataAccess _dataAccess;
        /// <summary>
        /// Validates Add UiPermission request
        /// </summary>
        public AddUiPermissionValidatorRequest(IUiPermissionDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            //1) Id
            //It is mandatory.
            //RuleFor(x => x.Id)
            //   .NotEmpty()
            //   .WithMessage(Resources.ErrorMessages.AddUiPermissionRequestValidator_Id_NotEmpty);

            //2) PermissionValue
            //It is mandatory.
            RuleFor(x => x.PermissionValue)
              .NotEmpty()
              .WithMessage(Resources.ErrorMessages.AddUiPermissionRequestValidator_PermissionValue_NotEmpty)

              //2.1 PermissionValue and PermissionParentId shouldn't be duplicate
              .MustAsync(IsUiPermissionValueDuplicate)
              .WithMessage(Resources.ErrorMessages.AddUiPermissionRequestValidator_UiPermission_MustUnique);

            //3) PermissionDisplayName
            //It is mandatory.
            RuleFor(x => x.PermissionDisplayName)
               .NotEmpty()
               .WithMessage(Resources.ErrorMessages.AddUiPermissionRequestValidator_PermissionDisplayName_NotEmpty);

            //4) PermissionTypeId
            //It is mandatory.
            RuleFor(x => x.PermissionTypeId)
               .NotEmpty()
               .WithMessage(Resources.ErrorMessages.AddUiPermissionRequestValidator_PermissionTypeId_NotEmpty);

            //5) ModuleId
            //It is mandatory.
            RuleFor(x => x.ModuleId)
               .NotEmpty()
               .WithMessage(Resources.ErrorMessages.AddUiPermissionRequestValidator_ModuleId_NotEmpty);
        }

        /// <summary>
        /// Is Ui PermissionValue Duplicate
        /// </summary>
        /// <param name="request"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>bool</returns>
        public async Task<bool> IsUiPermissionValueDuplicate(AddUiPermissionRequest request, string name, CancellationToken cancellationToken)
        {
            // get UiPermissions
            var permissions = await _dataAccess.GetUiPermissions();

            // Check for UiPermission if already exists{== put for a reason}
            if (permissions.Exists(x => x.PermissionValue.Equals(request.PermissionValue)
                                                   && x.PermissionParentId == request.PermissionParentId))
            {
                // if UiPermission already exist then return false
                return false;
            }
            return true;
        }
    }
}


