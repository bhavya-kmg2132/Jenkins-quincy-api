using System.Collections.Generic;


namespace Application.Users.Queries.Permission
{
    /// <summary>
    /// The ViewModel class is designed to store and manage UI-related data
    /// </summary>
    public class PermissionListVm
    {
        public IList<PermissionDto> PermissionList { get; internal set; }

    }
}
