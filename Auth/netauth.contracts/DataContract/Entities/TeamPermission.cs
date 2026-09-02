using NetAuth.Contract.DataContract.Common;
namespace NetAuth.Contract.DataContract.Entities
{
    public class TeamPermission
    {
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public string PermissionId { get; set; }
        public string PermissionValue { get; set; }
        public string PermissionDisplayName { get; set; }
        public string PermissionSetId { get; set; }
        public string PermissionSetName { get; set; }
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ApiName { get; set; }
    }
}
