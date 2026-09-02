namespace NetAuth.Domain.Entities.CoreRequests
{
    internal class AddPermission
    {
        public string PermissionId { get; set; }
        public string PermissionValue { get; set; }
        public string PermissionDisplayName { get; set; }
        public string PermissionSetId { get; set; }
        public string ModuleId { get; set; }
        public string PermissionType { get; set; }
        public bool IsActive { get; set; }
        public bool? IsAuthorized { get; set; }
        public string OwnerId { get; set; }
        public string SysData { get; set; }
        public string TenantId { get; set; }
        public string SubTenantId { get; set; }
        public string CreatedBy { get; set; }
    }
}
