namespace NetAuth.Contract.DataContract.Requests
{
    public class UpdateUiPermission
    {
        public string PermissionId { get; set; }
        public string PermissionDisplayName { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }
    }

}
