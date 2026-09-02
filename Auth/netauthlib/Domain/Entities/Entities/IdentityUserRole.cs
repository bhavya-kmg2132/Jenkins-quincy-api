using NetAuth.Domain.Common;

namespace NetAuth.Domain.Entities
{
    internal class IdentityUserRole : AuditableEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string RoleName { get; set; }
        public string RoleValue { get; set; }
    }
}
