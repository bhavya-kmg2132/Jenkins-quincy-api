namespace Gateway.Interface
{
    public interface IAuthorizationService
    {
        Task<bool> HasPermissionAsync(string userId, string route);

        Task<bool> SyncIdentityUserCacheAsync();
    }

}
