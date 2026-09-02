using NetAuth.Contract.DataContract.Enum;
using NetAuth.Contract.DataContract.Common;
namespace NetAuth.Contract.DataContract.Entities
{
    public class UserActivity : AuditableEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public DateTime? LastLogoutDateTime { get; set; }
        public DateTime? LastActivityDateTime { get; set; }
        public string LastActivityModule { get; set; }
        public UserActionType? LastActionType { get; set; }
        public string LastActivityDetail { get; set; }

    }
}
