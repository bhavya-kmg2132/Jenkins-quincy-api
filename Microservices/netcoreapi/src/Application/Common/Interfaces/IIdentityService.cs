using System.Threading.Tasks;
using NetAuth.Contract.DataContract.Dto;
using NetAuth.Contract.DataContract.Entities;

namespace Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<string> GetUserNameAsync(string userId);
        Task<bool> IsInRoleAsync(string userId, string role);
        Task<IdentityUser> GetIdentityUserAsync(string userId);
        Task<bool> AuthHasRequestPermissionAsync(string userId, string permissionName);
        Task<bool> SyncIdentityUserCacheAsync();
        Task<IdentityUser> ValidateIdentityUser(string username, string password);
        Task<bool> CreateUserAsync(string username, string password, string firstName, string lastName, string mobile, string oid, string auth_type);
        Task<string> CreateRefreshToken(string userId);
        Task<TokenModel> RefreshTokenAsync(string token);
        Task RevokeTokenAsync(string refreshToken);
    }
}
