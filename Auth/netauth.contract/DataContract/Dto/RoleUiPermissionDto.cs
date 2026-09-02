using NetAuth.Contract.DataContract.Common;
using NetAuth.Contract.DataContract.Entities;

namespace NetAuth.Contract.DataContract.Dto
{
    public class RoleUiPermissionDto : AuditableEntity
    {
        public string RoleId { get; set; }
        public UiPermission UiPermission { get; set; }
    }
}
