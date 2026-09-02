using NetAuth.Contract.DataContract.Common;
namespace NetAuth.Contract.DataContract.Entities
{
    public class UserPasswordHash : AuditableEntity
    {
        public string UserId { get; set; }
        public string PasswordHash { get; set; }
    }
}
