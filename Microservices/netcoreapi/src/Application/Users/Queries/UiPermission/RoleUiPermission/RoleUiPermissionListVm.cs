using System.Collections.Generic;


namespace Application.Users.Queries.UiPermission.RoleUiPermission
{
    /// <summary>
    /// The ViewModel class is designed to store and manage UI-related data
    /// </summary>
    public class RoleUiPermissionListVm
    {
        public IList<RoleUiPermissionDto> RoleUiPermissions { get; internal set; }
    }
}
