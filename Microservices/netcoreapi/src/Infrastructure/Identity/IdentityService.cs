using System;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Dapper.Extensions;
using NetAuth.Contract.DataContract.Dto;
using NetAuth.Contract.DataContract.Entities;


namespace Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IIdentityManager _identityManager;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenDataAccess _refreshTokenDataAccess;
        public IdentityService(IIdentityManager identityManager, IRefreshTokenDataAccess refreshTokenDataAccess, IJwtService jwtService)
        {
            _identityManager = identityManager;
            _refreshTokenDataAccess = refreshTokenDataAccess;
            _jwtService = jwtService;
        }


        public async Task<IdentityUser> ValidateIdentityUser(string username, string password)
        {
            if (username.IsNullOrWhiteSpace()) return null;
            //if (password.IsNullOrWhiteSpace()) return null;

            IdentityUser user = await _identityManager.ValidateIdentityUserAsync(username, password);

            return user;
        }

        public async Task<IdentityUser> GetIdentityUserAsync(string userId)
        {
            IdentityUser identityUser = await _identityManager.GetIdentityUserAsync(userId);

            return identityUser;
        }

        public async Task<bool> AuthHasRequestPermissionAsync(string userId, string permissionName)
        {
            bool identityUserHasPermisssion = await _identityManager.AuthHasRequestPermissionAsync(userId, permissionName);

            return identityUserHasPermisssion;
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            IdentityUser identityUser = await _identityManager.GetIdentityUserAsync(userId);

            return identityUser.UserName;
        }

        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            IdentityUser identityUser = await _identityManager.GetIdentityUserAsync(userId);

            return identityUser.UserRoles.Exists(r => r.RoleName.Equals(role));
        }

        public async Task<bool> SyncIdentityUserCacheAsync()
        {
            return await _identityManager.SyncIdentityUserCacheAsync();
        }

        public async Task<bool> CreateUserAsync(string username, string password, string firstName, string lastName, string mobile, string oid, string auth_type)
        {
            await _identityManager.CreateUserAsync(username, password, firstName, lastName, mobile, oid, auth_type);

            return true;
        }
        public async Task<TokenModel> RefreshTokenAsync(string token)
        {
            var storedToken = await _refreshTokenDataAccess.GetStoredTokenByRefreshToken(token);

            if (storedToken == null ||
                storedToken.IsRevoked ||
                storedToken.ExpiryDate < DateTime.UtcNow)
                return null;

            var user = await _identityManager.GetIdentityUserAsync(storedToken.UserId);

            var newAccessToken = _jwtService.GenerateToken(user);

            var newRefreshToken = await _refreshTokenDataAccess.RotateAsync(storedToken);

            return new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var storedToken = await _refreshTokenDataAccess.GetStoredTokenByRefreshToken(refreshToken);

            if (storedToken == null)
                return;

            // storedToken.IsRevoked = true;

            await _refreshTokenDataAccess.RevokeAsync(storedToken.Token);
        }
        public async Task<string> CreateRefreshToken(string userId)
        {
            var refreshToken = await _refreshTokenDataAccess.GenerateRefreshToken(userId);
            return refreshToken.Token;
        }
    }
}


//namespace Infrastructure.Identity
//{
//    public class IdentityService : IIdentityService
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
//        private readonly IAuthorizationService _authorizationService;

//        public IdentityService(
//            UserManager<ApplicationUser> userManager,
//            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
//            IAuthorizationService authorizationService)
//        {
//            _userManager = userManager;
//            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
//            _authorizationService = authorizationService;
//        }

//        public async Task<string> GetUserNameAsync(string userId)
//        {
//            var user = await _userManager.Users.FirstAsync(u => u.UserId == userId);

//            return user.UserName;
//        }

//        public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
//        {
//            var user = new ApplicationUser
//            {
//                UserName = userName,
//                Email = userName,
//            };

//            var result = await _userManager.CreateAsync(user, password);

//            return (result.ToApplicationResult(), user.UserId);
//        }

//        public async Task<bool> IsInRoleAsync(string userId, string role)
//        {
//            var user = _userManager.Users.SingleOrDefault(u => u.UserId == userId);

//            return await _userManager.IsInRoleAsync(user, role);
//        }

//        public async Task<bool> AuthorizeAsync(string userId, string policyName)
//        {
//            var user = _userManager.Users.SingleOrDefault(u => u.UserId == userId);

//            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

//            var result = await _authorizationService.AuthorizeAsync(principal, policyName);

//            return result.Succeeded;
//        }

//        public async Task<Result> DeleteUserAsync(string userId)
//        {
//            var user = _userManager.Users.SingleOrDefault(u => u.UserId == userId);

//            if (user != null)
//            {
//                return await DeleteUserAsync(user);
//            }

//            return Result.IsSuccess();
//        }

//        public async Task<Result> DeleteUserAsync(ApplicationUser user)
//        {
//            var result = await _userManager.DeleteAsync(user);

//            return result.ToApplicationResult();
//        }
//    }
//}
