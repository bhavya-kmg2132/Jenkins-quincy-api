using Application.Common.Mappings;

namespace Application.Users.Queries.UiPermission
{
    /// <summary>
    /// Dto class is used to pass data from domain to ViewModel layer.
    /// It helps in:
    /// 1. Abstraction of Domain layer
    /// 2. Data Hiding
    /// 3. Serialization and Lazy load problems
    /// </summary>
    public class UiPermissionDto : IMapFrom<NetAuth.Contract.DataContract.Entities.UiPermission>
    {
        public string PermissionId { get; set; }
        public string PermissionValue { get; set; }
        public string PermissionDisplayName { get; set; }
        public string PermissionTypeId { get; set; }
        public string PermissionTypeName { get; set; }
        public string PermissionParentId { get; set; }
        public string PermissionParentName { get; set; }
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool IsActive { get; set; }


        //Mapping UiPermissionDto with UiPermission entity
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NetAuth.Contract.DataContract.Entities.UiPermission, UiPermissionDto>();
        }
    }
}
