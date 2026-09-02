using NetAuth.Domain.Dto;
using NetAuth.Domain.Entities;
using NetAuth.Lib.Domain.Entities.Entities;

namespace NetAuth.Interfaces
{
    internal interface IIdentityManager
    {
        Task<List<Permission>> GetPermissionsAsync();
        Task<List<Permission>> GetAllPermissionsAsync();
        Task<RoleDto> GetPermissionsForRoleAsync(string roleId);
        Task<List<UserDto>> GetUserByRoleIdAsync(string roleId);
        Task<IdentityUser> GetIdentityUserAsync(string userName_userId_userOid);
        Task<bool> AuthHasRequestPermissionAsync(string userId, string permissionValue);
        Task<List<UserDto>> GetUsersAsync();
    }
}
