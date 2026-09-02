namespace NetAuth.Domain.Entities
{
    internal class UserPermission
    {
        public string UserId { get; set; }
        public string PermissionId { get; set; }
        public string PermissionValue { get; set; }
        public string PermissionDisplayName { get; set; }
        public string PermissionSetId { get; set; }
        public string PermissionSetName { get; set; }
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ApiName { get; set; }
        public string PermissionType { get; set; }
        public string ActionPermissionEndPoint { get; set; }
        public bool IsActive { get; set; }
    }
}
