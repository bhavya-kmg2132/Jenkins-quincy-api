using NetAuth.Domain.Common;

namespace NetAuth.Lib.Domain.Entities.Entities
{
    internal class IdentityUserUiPermission : AuditableEntity
    {
        public string UserId { get; set; }
        public string PermissionId { get; set; }
        public string PermissionValue { get; set; }
        public string PermissionDisplayName { get; set; }
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string PermissionTypeId { get; set; }
        public string PermissionTypeName { get; set; }
        public string PermissionParentId { get; set; }
        public string PermissionParentName { get; set; }
    }
}
