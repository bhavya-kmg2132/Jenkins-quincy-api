

using NetAuth.Contract.DataContract.Entities;

namespace NetAuth.Contract.DataContract.Requests
{
    public class AddUiPermissionsForRole
    {
        public RoleUiPermission RoleUiPermission { get; set; }
        public string CreatedBy { get; set; }
    }
}
