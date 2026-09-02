namespace NetAuth.Contract.DataContract.Requests
{
    public class AddUiPermission
    {
        public string PermissionId { get; set; }
        public string PermissionValue { get; set; }
        public string PermissionDisplayName { get; set; }
        public string PermissionTypeId { get; set; }
        public string PermissionParentId { get; set; }
        public string ModuleId { get; set; }
        public string CreatedBy { get; set; }

    }

}
