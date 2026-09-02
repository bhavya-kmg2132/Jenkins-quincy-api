using NetAuth.Contract.DataContract.Entities;

namespace netauthlib
{
    public interface INetAuthProvider
    {
        Task<List<NetAuth.Contract.DataContract.Entities.Permission>> GetPermissionsAsync();
        Task<List<NetAuth.Contract.DataContract.Entities.Permission>> GetAllPermissionsAsync();
        Task<List<NetAuth.Contract.DataContract.Dto.RoleDto>> GetRoles();
        Task<List<NetAuth.Contract.DataContract.Dto.UserDto>> GetUsersAsync();
        Task<NetAuth.Contract.DataContract.Dto.UserDto> GetUserFromDb(string userId);
        Task<NetAuth.Contract.DataContract.Dto.UserDto> GetUserById(string userId);
        Task<IdentityUser> GetIdentityUserByUserName(string userName_userId_userOid);
        Task<NetAuth.Contract.DataContract.Dto.UserDto> GetUserVmByUserName(string userName);
        Task<NetAuth.Contract.DataContract.Dto.UserDto> GetUserByOid(string oid);
        Task<NetAuth.Contract.DataContract.Dto.UserDto> GetUserByUserId(string userId);
        Task<List<NetAuth.Contract.DataContract.Dto.UserDto>> GetUserByRoleId(string roleId);
        Task<List<NetAuth.Contract.DataContract.Entities.AuthReferenceLookup>> GetAuthReferenceLookupList(string type);

        #region UiPermission
        Task<List<NetAuth.Contract.DataContract.Dto.RoleUiPermissionDto>> GetUiPermissionsForRole(string roleId);
        Task<List<NetAuth.Contract.DataContract.Entities.UiPermission>> GetUiPermissions();
        Task<string> AddUiPermission(NetAuth.Contract.DataContract.Requests.AddUiPermission addUiPermission);
        Task<bool> UpdateUiPermission(NetAuth.Contract.DataContract.Requests.UpdateUiPermission updateUiPermission);
        Task<bool> AddUiPermissionsForRole(NetAuth.Contract.DataContract.Requests.AddUiPermissionsForRole addUiPermissionForRole);
        #endregion

        #region UserActivity
        Task<string> AddUserActivity(NetAuth.Contract.DataContract.Requests.AddUserActivity addUserActivity);
        Task<List<NetAuth.Contract.DataContract.Dto.UserActivityDto>> GetUserActivities(string userId, int pageSize, int pageNumber, string startDate, string endDate);
        Task<List<NetAuth.Contract.DataContract.Entities.UserActivity>> GetUserActivitiesByUserIds(List<string> userIds);
        #endregion

        Task<bool> AddPermissionsGrantedForUser(string userId, List<string> permissionIds, string createdBy);
        Task<bool> AddPermissionsDeniedForUser(string userId, List<string> permissionIds, string createdBy);
        Task<bool> AddPermissionsForRole(string roleId, List<string> permissionIds, string createdBy);
        Task<string> AddUser(NetAuth.Contract.DataContract.Requests.CreateUserRequest request);
        Task<bool> AddRole(string userId, string roleId, string createdBy);
        Task<bool> DeleteRole(string userId, string roleId, string createdBy);
        Task<bool> AddRoles(string userId, List<string> roleIds, string createdBy);
        Task<NetAuth.Contract.DataContract.Dto.RoleDto> GetPermissionsByRoleId(string roleId);
        Task<bool> AddPermission(NetAuth.Contract.DataContract.Requests.AddPermission permission, string userName);
        Task<bool> UpdatePermission(NetAuth.Contract.DataContract.Requests.UpdatePermission editPermission, string userName);
        Task<int> UpdateUserPasswordHash(NetAuth.Contract.DataContract.Requests.UpdateUserPasswordHash request);
        Task<NetAuth.Contract.DataContract.Entities.UserPasswordHash> GetUserPasswordHash(string userId);
        Task<int> UpdateUser(NetAuth.Contract.DataContract.Requests.UpdateUser updateUser);
        Task<int> ActivateOrInActivateUser(NetAuth.Contract.DataContract.Requests.ActivateOrInActivateUser activateOrInActivateUser);
        Task<List<NetAuth.Contract.DataContract.Dto.UsersDto>> GetUsersByStatus(string status);

        #region Team management
        Task<List<NetAuth.Contract.DataContract.Dto.TeamDto>> GetTeams();
        Task<NetAuth.Contract.DataContract.Dto.TeamDto> GetTeamById(string teamId);
        Task<string> AddTeam(NetAuth.Contract.DataContract.Dto.TeamDto team, string createdBy);
        Task<bool> AddTeamMembers(string teamId, List<string> userIds, string createdBy);
        Task<bool> RemoveTeamMember(string teamId, string userId);
        Task<List<NetAuth.Contract.DataContract.Dto.TeamDto>> GetTeamsByUserId(string userId);
        Task<List<NetAuth.Contract.DataContract.Dto.TeamMemberDto>> GetTeamMembersByTeamId(string teamId);
        #endregion
    }
}
