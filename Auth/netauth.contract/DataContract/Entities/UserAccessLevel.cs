using NetAuth.Contract.DataContract.Common;
namespace NetAuth.Contract.DataContract.Entities
{
    public class UserAccessLevel : AuditableEntity
    {
        public string UserAccessLevelValue { get; set; }
        public string UserAccessLevelName { get; set; }
    }
}
