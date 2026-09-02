using NetAuth.Domain.Common;
using NetAuth.Domain.Entities;

namespace NetAuth.Domain.Dto
{
    internal class UserUiPermissionDto : AuditableEntity
    {
        public string UserId { get; set; }
        public UiPermission UiPermission { get; set; }
     
    }
}
