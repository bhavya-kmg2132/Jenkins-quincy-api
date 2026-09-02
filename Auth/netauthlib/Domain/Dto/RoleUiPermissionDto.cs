using NetAuth.Domain.Common;
using NetAuth.Domain.Entities;

namespace NetAuth.Domain.Dto
{
    internal class RoleUiPermissionDto : AuditableEntity
    {
        public string RoleId { get; set; }
        public UiPermission UiPermission { get; set; }
    }
}
