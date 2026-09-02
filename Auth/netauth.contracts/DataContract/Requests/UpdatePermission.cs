
namespace NetAuth.Contract.DataContract.Requests
{
    public class UpdatePermission 
    {
        public string Id { get; set; }
        public string PermissionValue { get; set; }
        public string PermissionDisplayName { get; set; }
        public string PermissionSetId { get; set; }
        public string PermissionSetName { get; set; }
        public string PermissionType { get; set; }
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ApiName { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        public string ApproverId { get; set; }
        public DateTime? ApprovedDateTime { get; set; }
        public bool? IsAuthorized { get; set; }
        public string AuthorizedById { get; set; }
        public DateTime? AuthorizedDateTime { get; set; }

    }
}
