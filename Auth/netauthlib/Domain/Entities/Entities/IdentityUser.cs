using NetAuth.Domain.Common;
using NetAuth.Domain.Entities;

namespace NetAuth.Lib.Domain.Entities.Entities
{
    internal class IdentityUser : AuditableEntity
    {
        /// <summary>
        /// This time stamp property is required/mandatory for any class to add into caching
        /// </summary>
        public DateTime CacheTimeStamp { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string auth_type { get; set; } // "AzureAD" or "Database"

        public string oid { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string BusinessUnit { get; set; }

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
        public string LastLoginTime { get; set; }
        public string TimeZone { get; set; }


        /// <summary>
        /// Roles assigned to this user
        /// </summary>
        public List<IdentityUserRole> UserRoles { get; set; }

        /// <summary>
        /// User permissions = Role permissions + granted permissions
        /// </summary>
        public List<Permission> UserPermissions { get; set; }

        public string AccessLevel { get; set; }

        public string UserAccessLevelName { get; set; }
        public string ProducerLevel { get; set; }
        public DateTime? ProducerLevelDate { get; set; }

        public string RegionCode { get; set; }
    }
}
