using NetAuth.Contract.DataContract.Common;

namespace NetAuth.Contract.DataContract.Entities
{
    public class IdentityUserRole : AuditableEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string RoleName { get; set; }
        public string RoleValue { get; set; }
    }
}
