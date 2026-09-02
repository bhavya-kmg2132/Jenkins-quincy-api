using NetAuth.Domain.Dto;
using NetAuth.Domain.Entities;
using NetAuth.Domain.Entities.CoreRequests;
using System.Collections.Generic;

namespace NetAuth.Interfaces
{
    internal interface IUserDataAccess
    {
        Task<string> AddUser(CreateUserRequest user);
        Task<UserDto> GetUserFromDbAsync(string userId);
        Task<string> GetUserIdBasedOnOidFromDb(string oid);
        Task<RoleDto> GetPermissionsForRoleAsync(string roleId);
        Task<List<RoleDto>> GetRoles();
        Task<RoleDto> GetRoleById(string roleId);
        Task<bool> AddRolesForUser(string userId, List<string> roleIds, string createdBy);
        Task<bool> UpdateUserAccessLevel(User user);
        Task<List<UserAccessLevel>> GetUserAccessLevelList();
        Task<bool> AddPermissionsDeniedForUser(string userId, List<string> permissionIds, string createdBy);
        Task<bool> AddPermissionsGrantedForUser(string userId, List<string> permissionIds, string createdBy);
        Task<bool> AddPermissionsForRole(string roleId, List<string> permissionIds, string createdBy);
        Task<List<AuthReferenceLookup>> GetAuthReferenceLookupList(string type);
        Task<bool> AddPermission(Permission permission, string userName);
        Task<bool> UpdatePermission(Domain.Entities.UpdatePermission permission, string userName);

        //User Profile
        Task<List<UserProfile>> GetUserProfileByUserId(string userId);
        Task<List<UserProfile>> GetUserProfileByProfileId(string profileId);
        Task<List<UserProfile>> GetUserProfileList();
        Task<bool> AddPermissions(string permissionValue, string permissionDisplayName, string userName);
        Task<bool> AddRoleForUser(string userId, string roleId, string createdBy);
        Task<bool> DeleteRoleForUser(string userId, string roleId, string createdBy);
        Task<List<UserUiPermissionDto>> GetUserUiPermissionsByUserId(string userId);
        Task<int> UpdateUserPasswordHash(UserPasswordHash userPasswordHash);
        Task<int> UpdateUser(Domain.Entities.User updateUser);
        Task<int> ActivateOrInActivateUser(string userId, bool isActive);

        #region UserActivity 
        Task<string> AddUserActivity(UserActivity userActivity);
        Task<List<UserActivityDto>> GetUserActivities(string userId, int pageSize, int pageNumber, string startDate, string endDate);
        Task<List<UserActivity>> GetUserActivitiesByUserIds(string userIds);
        Task<UserPasswordHash> GetUserPasswordHash(string userId);

        #endregion

        Task<List<UsersDto>> GetUsersByStatus(string status);

        // Team management
        Task<List<TeamDto>> GetTeams();
        Task<TeamDto> GetTeamById(string teamId);
        Task<string> AddTeam(TeamDto team, string createdBy);
        Task<bool> AddTeamMembers(string teamId, List<string> userIds, string createdBy);
        Task<bool> RemoveTeamMember(string teamId, string userId);
        Task<List<TeamDto>> GetTeamsByUserId(string userId);
        Task<List<NetAuth.Domain.Dto.TeamMemberDto>> GetTeamMembersByTeamId(string teamId);

    }
}
