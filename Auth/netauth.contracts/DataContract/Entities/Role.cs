using NetAuth.Contract.DataContract.Common;
namespace NetAuth.Contract.DataContract.Entities
{
    public class Role : AuditableEntity
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string RoleValue { get; set; }
    }
}
