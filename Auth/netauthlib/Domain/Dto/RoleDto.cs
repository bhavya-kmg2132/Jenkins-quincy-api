using NetAuth.Domain.Common;
using NetAuth.Domain.Entities;

namespace NetAuth.Domain.Dto
{
    internal class RoleDto : AuditableEntity
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string RoleValue { get; set; }
        public List<Permission> RolePermissions { get; set; }
    }
}
