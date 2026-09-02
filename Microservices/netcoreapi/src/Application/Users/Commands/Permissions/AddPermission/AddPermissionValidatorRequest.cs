using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Users.Commands.UserPermissions.AddPermissionRequest;

namespace Application.Users.Commands.Permissions
{
    public class AddPermissionValidatorRequest : AbstractValidator<AddPermissionRequest>
    {
        private readonly IIdentityManager _dataAccess;
        /// <summary>
        /// Validates Add Permission request
        /// </summary>
        public AddPermissionValidatorRequest(IIdentityManager dataAccess)
        {
            _dataAccess = dataAccess;
            //1) Id
            //It is mandatory.
            //RuleFor(x => x.Id)
            //   .NotEmpty()
            //   .WithMessage(Resources.ErrorMessages.AddPermissionRequestValidator_Id_NotEmpty);

            //2) PermissionValue
            //It is mandatory.
            RuleFor(x => x.PermissionValue)
              .NotEmpty()
              .WithMessage(Resources.ErrorMessages.AddPermissionRequestValidator_PermissionValue_NotEmpty)

              //2.1 PermissionValue  and DisplayName shouldn't be duplicate
              .MustAsync(IsPermissionValueDuplicate)
              .WithMessage(Resources.ErrorMessages.AddPermissionRequestValidator_Permission_MustUnique);

            //3) PermissionDisplayName
            //It is mandatory.
            RuleFor(x => x.PermissionDisplayName)
               .NotEmpty()
               .WithMessage(Resources.ErrorMessages.AddPermissionRequestValidator_PermissionDisplayName_NotEmpty);

            //4) PermissionSetId
            //It is mandatory.
            RuleFor(x => x.PermissionSetId)
               .NotEmpty()
               .WithMessage(Resources.ErrorMessages.AddPermissionRequestValidator_PermissionSetId_NotEmpty);

            //5) ModuleId
            //It is mandatory.
            RuleFor(x => x.ModuleId)
               .NotEmpty()
               .WithMessage(Resources.ErrorMessages.AddPermissionRequestValidator_ModuleId_NotEmpty);
        }

        /// <summary>
        /// IsPermissionValueDuplicate
        /// </summary>
        /// <param name="request"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>bool</returns>
        public async Task<bool> IsPermissionValueDuplicate(AddPermissionRequest request, string name, CancellationToken cancellationToken)
        {
            // get permissions
            var permissions = await _dataAccess.GetPermissionsAsync();

            // Check for permission if already exists, and not for this id
            if (permissions.Exists(x => x.PermissionValue.Equals(request.PermissionValue)
                                                   && x.PermissionDisplayName.Equals(request.PermissionDisplayName)))
            {
                // if permission already exist then return false
                return false;
            }
            return true;
        }
    }
}


