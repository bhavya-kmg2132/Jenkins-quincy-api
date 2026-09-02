using System.Collections.Generic;
using Application.Query.UiPermission.UserUiPermission;


namespace Application.Users.Queries.UiPermission.UserUiPermission
{
    /// <summary>
    /// The ViewModel class is designed to store and manage UI-related data
    /// </summary>
    public class UserUiPermissionListVm
    {
        public IList<UserUiPermissionDto> UserUiPermissions { get; internal set; }
    }
}
