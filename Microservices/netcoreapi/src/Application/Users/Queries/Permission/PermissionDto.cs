using Application.Common.Mappings;

namespace Application.Users.Queries.Permission
{
    /// <summary>
    /// Dto class is used to pass data from domain to ViewModel layer.
    /// It helps in:
    /// 1. Abstraction of Domain layer
    /// 2. Data Hiding
    /// 3. Serialization and Lazy load problems
    /// </summary>
    public class PermissionDto : IMapFrom<NetAuth.Contract.DataContract.Entities.Permission>
    {
        public string PermissionId { get; set; }
        public string PermissionValue { get; set; }
        public string PermissionDisplayName { get; set; }
        public string PermissionSetId { get; set; }
        public string PermissionSetName { get; set; }
        public string PermissionType { get; set; }
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ApiName { get; set; }
        public string ActionPermissionEndPoint { get; set; }
        public bool IsActive { get; set; }


        //Mapping PermissionDto with Permission entity
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NetAuth.Contract.DataContract.Entities.Permission, PermissionDto>();
        }
    }
}
