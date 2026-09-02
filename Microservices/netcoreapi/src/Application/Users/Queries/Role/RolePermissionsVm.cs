using System.Collections.Generic;

namespace Application.Users.Queries.Role
{
    /// <summary>
    /// Dto class is used to pass data from domain to ViewModel layer.
    /// It helps in:
    /// 1. Abstraction of Domain layer
    /// 2. Data Hiding
    /// 3. Serialization and Lazy load problems
    /// </summary>
    public class RolePermissionsVm
    {
        public NetAuth.Contract.DataContract.Entities.Role Role { get; set; }
        public List<NetAuth.Contract.DataContract.Entities.Permission> Permissions { get; set; }
    }
}
