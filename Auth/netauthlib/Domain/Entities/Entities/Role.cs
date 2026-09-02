using NetAuth.Domain.Common;

namespace NetAuth.Lib.Domain.Entities
{
    internal class Role : AuditableEntity
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string RoleValue { get; set; }
    }
}
