using System.Collections.Generic;
using System.Threading.Tasks;
using Application.SystemManager.UpdateActionPermissionEndPoint;
using Domain.Entities;
using NetAuth.Contract.DataContract.Dto;
using NetAuth.Contract.DataContract.Entities;
using NetAuth.Contract.DataContract.Requests;

namespace Application.Common.Interfaces
{
    public interface IUserDataAccess
    {
        Task<UserDto> GetUserFromNetAuthLibAsync(string username);

        Task<string> AddUser(CreateUserRequest request);
        Task<UserDto> GetUserFromDbAsync(string userId);
        Task<string> GetUserIdBasedOnOidFromDb(string oid);
        Task<RoleDto> GetPermissionsForRoleAsync(string roleId);
        Task<List<RoleDto>> GetRoles();
        Task<RoleDto> GetRoleById(string roleId);
        Task<bool> AddRolesForUser(string userId, List<string> roleIds, string createdBy);
        Task<bool> UpdateUserAccessLevel(UserDto user);
        Task<List<UserAccessLevel>> GetUserAccessLevelList();
        Task<bool> AddPermissionsDeniedForUser(string userId, List<string> permissionIds, string createdBy);
        Task<bool> AddPermissionsGrantedForUser(string userId, List<string> permissionIds, string createdBy);
        Task<bool> AddPermissionsForRole(string roleId, List<string> permissionIds, string createdBy);
        Task<List<AuthReferenceLookup>> GetAuthReferenceLookupList(string type);
        Task<bool> AddPermission(AddPermission permission, string userName);
        Task<bool> UpdatePermission(NetAuth.Contract.DataContract.Requests.UpdatePermission permission, string userName);

        Task<List<UserProfile>> GetUserProfileByUserId(string userId);
        Task<List<UserProfile>> GetUserProfileByProfileId(int profileId);
        Task<List<UserProfile>> GetUserProfileList();
        Task<bool> AddPermissions(string permissionValue, string permissionDisplayName, string userName);
        Task<bool> AddRoleForUser(string userId, string roleId, string createdBy);
        Task<bool> DeleteRoleForUser(string userId, string roleId, string updatedBy);
        Task<List<UserUiPermissionDto>> GetUserUiPermissionsByUserId(string userId);
        Task<int> UpdateUserPasswordHash(UpdateUserPasswordHash updateUserPasswordHash);
        Task<UserPasswordHash> GetUserPasswordHash(string userId);
        Task<int> UpdateUser(UpdateUser updateUser);
        Task<int> ActivateOrInActivateUser(ActivateOrInActivateUser activateOrInActivateUser);

        #region UserActivity
        Task<string> AddUserActivity(AddUserActivity userActivity);
        Task<List<UserActivityDto>> GetUserActivities(string userId, int pageSize, int pageNumber, int period);
        Task<List<UserActivity>> GetUserActivitiesByUserIds(List<string> userIds);
        #endregion

        Task<List<UserName>> GetUserFullName();
        Task<List<UsersDto>> GetUsersByStatus(string status);

        #region Team management
        Task<List<TeamDto>> GetTeams();
        Task<TeamDto> GetTeamById(string teamId);
        Task<string> AddTeam(TeamDto team, string createdBy);
        Task<bool> AddTeamMembers(string teamId, List<string> userIds, string createdBy);
        Task<bool> RemoveTeamMember(string teamId, string memberId);
        Task<List<TeamDto>> GetTeamsByUserId(string userId);
        Task<List<TeamMemberDto>> GetTeamMembersByTeamId(string teamId);
        #endregion

        Task AddActionPermissionEndPoint(List<UpdateActionPermissionEndPointDto> items, string moduleId, string permissionSetId, string permissionType, string apiName, string createdBy);
        Task<List<string>> UpdateCodePermissionActionEndpoints(List<UpdateActionPermissionEndPointDto> items, string updatedBy);
    }
}
