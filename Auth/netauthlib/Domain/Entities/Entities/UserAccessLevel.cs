using NetAuth.Domain.Common;

namespace NetAuth.Domain.Entities
{
    internal class UserAccessLevel : AuditableEntity
    {
        public string UserAccessLevelValue { get; set; }
        public string UserAccessLevelName { get; set; }
    }
}
