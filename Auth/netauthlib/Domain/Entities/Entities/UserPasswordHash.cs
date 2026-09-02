using NetAuth.Domain.Common;

namespace NetAuth.Domain.Entities
{
    internal class UserPasswordHash : AuditableEntity
    {
        public string UserId { get; set; }
        public string PasswordHash { get; set; }
    }
}
