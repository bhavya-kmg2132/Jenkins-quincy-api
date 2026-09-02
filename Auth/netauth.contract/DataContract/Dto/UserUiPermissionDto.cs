using NetAuth.Contract.DataContract.Common;
using NetAuth.Contract.DataContract.Entities;

namespace NetAuth.Contract.DataContract.Dto
{
    public class UserUiPermissionDto : AuditableEntity
    {
        public string UserId { get; set; }
        public UiPermission UiPermission { get; set; }
     
    }
}
