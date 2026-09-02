using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IUiPermissionDataAccess
    {
        #region IUiPermissionDataAccess

        Task<List<NetAuth.Contract.DataContract.Entities.UiPermission>> GetUiPermissions();
        Task<List<NetAuth.Contract.DataContract.Dto.RoleUiPermissionDto>> GetUiPermissionsForRole(string roleId);
        Task<bool> AddUiPermissionsForRole(NetAuth.Contract.DataContract.Requests.AddUiPermissionsForRole addUiPermissionsForRole);
        Task<string> AddUiPermission(NetAuth.Contract.DataContract.Requests.AddUiPermission addUiPermission);
        Task<bool> UpdateUiPermission(NetAuth.Contract.DataContract.Requests.UpdateUiPermission updateUiPermission);
        Task ResetUiPermissionCache();

        #endregion
    }
}
