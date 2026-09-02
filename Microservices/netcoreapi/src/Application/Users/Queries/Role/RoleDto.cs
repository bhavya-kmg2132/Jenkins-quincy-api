using System.Collections.Generic;
using Application.Common.Mappings;

namespace Application.Role.Queries.GetRole
{
    /// <summary>
    /// Dto class is used to pass data from domain to ViewModel layer.
    /// It helps in:
    /// 1. Abstraction of Domain layer
    /// 2. Data Hiding
    /// 3. Serialization and Lazy load problems
    /// </summary>
    public class RoleDto : IMapFrom<NetAuth.Contract.DataContract.Dto.RoleDto>
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string RoleValue { get; set; }
        public List<NetAuth.Contract.DataContract.Entities.Permission> RolePermissions { get; set; }

        //Mapping RoleDto with Role entity
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NetAuth.Contract.DataContract.Dto.RoleDto, RoleDto>();
        }
    }
}
