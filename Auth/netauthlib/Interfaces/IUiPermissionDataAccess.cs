using NetAuth.Domain.Dto;
using NetAuth.Domain.Entities;
using System.Collections.Generic;

namespace NetAuth.Interfaces
{
    internal interface IUiPermissionDataAccess
    {
        #region IUiPermissionDataAccess
        Task<List<UiPermission>> GetUiPermissions();
        Task<List<RoleUiPermissionDto>> GetUiPermissionsForRole(string roleId);
        Task<bool> AddUiPermissionsForRole(List<RoleUiPermissionDto> uiPermissionsForRoles);
        Task<string> AddUiPermission(UiPermission UiPermission);
        Task<bool> ActivateUiPermission(UiPermission UiPermission);
        #endregion
    }
}
