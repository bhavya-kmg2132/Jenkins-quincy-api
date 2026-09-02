using NetAuth.Contract.DataContract.Common;
using NetAuth.Contract.DataContract.Entities;

namespace NetAuth.Contract.DataContract.Dto
{
    public class RoleDto : AuditableEntity
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string RoleValue { get; set; }
        public List<Permission> RolePermissions { get; set; }
    }
}
