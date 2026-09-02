namespace NetAuth.Domain.Entities.CoreRequests
{
    internal class UpdateUiPermission
    {
        public string PermissionId { get; set; }
        public string PermissionDisplayName { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }
    }

}
