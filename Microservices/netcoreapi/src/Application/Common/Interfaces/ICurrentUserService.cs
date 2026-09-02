using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        List<string> UserRoles { get; set; }
        public string preferred_username { get; set; }
        public string oid { get; set; }
        public string Scope { get; set; }
        public string name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string BusinessUnit { get; set; }
        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string AccessLevel { get; set; }
        public string display_name { get; set; }
        public string CorrelationId { get; }

        public string RequestId { get; }

        public string RequestOid { get; }

        Task<bool> ValidateUserToken();
        Task<bool> ValidateRequestUser();
        Task<bool> HasPermissionAsync(string permissionName);
    }
}
