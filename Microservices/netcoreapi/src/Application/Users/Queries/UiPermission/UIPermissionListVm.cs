using System.Collections.Generic;


namespace Application.Users.Queries.UiPermission
{
    /// <summary>
    /// The ViewModel class is designed to store and manage UI-related data
    /// </summary>
    public class UiPermissionListVm
    {
        public IList<UiPermissionDto> UiPermissionList { get; internal set; }
    }
}
