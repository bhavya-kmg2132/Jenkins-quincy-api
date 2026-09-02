using System.Collections.Generic;
using System.Threading.Tasks;
using NetAuth.Contract.DataContract.Entities;

namespace Application.Common.Interfaces
{
    public interface IIdentityManager
    {
        Task<List<NetAuth.Contract.DataContract.Entities.Permission>> GetPermissionsAsync();
        Task<List<NetAuth.Contract.DataContract.Entities.Permission>> GetAllPermissionsAsync();
        Task ResetUserCache(List<string> userIds = null);
        Task<IdentityUser> GetIdentityUserAsync(string userName_userId_userOid);
        Task<bool> AuthHasRequestPermissionAsync(string userId, string permissionValue);
        Task<List<NetAuth.Contract.DataContract.Dto.UserDto>> GetUsersAsync();
        Task<bool> NightlyCacheResetAsync();
        Task ResetIdentityUserCache(string userName_userId_userOid);
        Task<bool> SyncIdentityUserCacheAsync();
        Task<List<NetAuth.Contract.DataContract.Dto.UserDto>> GetUserByRoleIdAsync(string roleId);
        Task<IdentityUser> ValidateIdentityUserAsync(string username, string password);
        Task<string> CreateUserAsync(string username, string password, string firstName, string lastName, string mobile, string oid, string auth_type);
    }
}
