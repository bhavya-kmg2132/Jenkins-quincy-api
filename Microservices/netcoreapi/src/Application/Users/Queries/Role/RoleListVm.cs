using System.Collections.Generic;
using Application.Role.Queries.GetRole;


namespace Application.Users.Queries
{
    /// <summary>
    /// The ViewModel class is designed to store and manage UI-related data
    /// </summary>
    public class RoleListVm
    {
        public IList<RoleDto> RolesList { get; internal set; }
    }
}
