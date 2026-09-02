using NetAuth.Contract.DataContract.Common;
using NetAuth.Contract.DataContract.Entities;

namespace NetAuth.Contract.DataContract.Dto
{
    public class UserDto : AuditableEntity
    {
        public string Id { get; set; }
        public string EmpId { get; set; }
        public string EmpType { get; set; }
        public string UserRoleId { get; set; }
        public string auth_type { get; set; } // "AzureAD" or "Database"
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string BusinessUnit { get; set; }
        public string oid { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string preferred_username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondaryEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string Extension { get; set; }
        public string display_name { get; set; }
        public string ManagerId { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Organization { get; set; }

        /// <summary>
        /// User Roles
        /// </summary>
        public List<RoleDto> Roles { get; set; } = new();

        /// <summary>
        /// Teams this user belongs to
        /// </summary>
        public List<TeamDto> Teams { get; set; } = new();

        /// <summary>
        /// Permissions granted to User
        /// </summary>
        public List<Permission> PermissionsGranted { get; set; } = new();

        /// <summary>
        /// Permissions denied to User
        /// </summary>
        public List<Permission> PermissionsDenied { get; set; } = new();

        /// <summary>
        /// User permissions = Role permissions + granted permissions
        /// </summary>
        public List<Permission> UserPermissions { get; set; } = new();

        /// <summary>
        /// UserUiPermission
        /// </summary>
        public List<UserUiPermissionDto> UserUiPermissions { get; set; } = new();
        public string AccessLevel { get; set; }

    }
}
