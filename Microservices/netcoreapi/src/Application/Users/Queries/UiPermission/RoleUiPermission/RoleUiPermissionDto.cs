using Application.Common.Mappings;

namespace Application.Users.Queries.UiPermission.RoleUiPermission
{
    /// <summary>
    /// Dto class is used to pass data from domain to ViewModel layer.
    /// It helps in:
    /// 1. Abstraction of Domain layer
    /// 2. Data Hiding
    /// 3. Serialization and Lazy load problems
    /// </summary>
    public class RoleUiPermissionDto : IMapFrom<NetAuth.Contract.DataContract.Dto.RoleUiPermissionDto>
    {
        public string RoleId { get; set; }
        public string PermissionId { get; set; }
        public string PermissionValue { get; set; }
        public string PermissionDisplayName { get; set; }
        public string PermissionTypeId { get; set; }
        public string PermissionTypeName { get; set; }
        public string PermissionParentId { get; set; }
        public string PermissionParentName { get; set; }
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }

        //Mapping RoleUiPermissionDto with RoleUiPermission entity
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NetAuth.Contract.DataContract.Dto.RoleUiPermissionDto, RoleUiPermissionDto>()
                .ForMember(d => d.PermissionId, opt => opt.MapFrom(e => e.UiPermission.PermissionId))
                .ForMember(d => d.PermissionValue, opt => opt.MapFrom(e => e.UiPermission.PermissionValue))
                .ForMember(d => d.PermissionDisplayName, opt => opt.MapFrom(e => e.UiPermission.PermissionDisplayName))
                .ForMember(d => d.ModuleId, opt => opt.MapFrom(e => e.UiPermission.ModuleId))
                .ForMember(d => d.ModuleName, opt => opt.MapFrom(e => e.UiPermission.ModuleName))
                .ForMember(d => d.PermissionTypeId, opt => opt.MapFrom(e => e.UiPermission.PermissionTypeId))
                .ForMember(d => d.PermissionTypeName, opt => opt.MapFrom(e => e.UiPermission.PermissionTypeName))
                .ForMember(d => d.PermissionParentId, opt => opt.MapFrom(e => e.UiPermission.PermissionParentId))
                .ForMember(d => d.PermissionParentName, opt => opt.MapFrom(e => e.UiPermission.PermissionParentName));
        }
    }
}
