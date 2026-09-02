using System.Collections.Generic;

namespace Application.SystemManager.UpdateActionPermissionEndPoint
{
    public class UpdateActionPermissionEndPointVm
    {
        public int AddedCount { get; set; }
        public int SkippedCount { get; set; }
        public int ConflictedCount { get; set; }
        public int UpdatedActionEndpointCount { get; set; }
        public List<string> AddedPermissionValues { get; set; } = new List<string>();
        public List<string> SkippedPermissionValues { get; set; } = new List<string>();
        public List<string> ConflictedPermissionValues { get; set; } = new List<string>();
        public List<string> UpdatedActionEndpointPermissionValues { get; set; } = new List<string>();
    }
}
