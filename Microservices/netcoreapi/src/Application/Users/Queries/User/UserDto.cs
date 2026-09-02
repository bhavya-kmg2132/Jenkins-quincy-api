using System.Collections.Generic;
using Application.Common.Mappings;
using Application.Users.Queries.User;
using NetAuth.Domain.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.Users.Queries
{
    /// <summary>
    /// Dto class is used to pass data from domain to ViewModel layer.
    /// It helps in:
    /// 1. Abstraction of Domain layer
    /// 2. Data Hiding
    /// 3. Serialization and Lazy load problems
    /// </summary>
    public class UserDto : IMapFrom<NetAuth.Contract.DataContract.Dto.UserDto>
    {
        public string UserId { get; set; }
        public string EmpId { get; set; }
        public string EmpType { get; set; }
        public string auth_type { get; set; } // "AzureAD" or "Database"

        public string UserName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string BusinessUnit { get; set; }
        public bool IsDeleted { get; set; }
        public string oid { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string preferred_username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondaryEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string Extension { get; set; }
        public string display_name { get; set; }
        public string ManagerId { get; set; }
        public string UserRoleId { get; set; }
        public bool? IsActive { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Organization { get; set; }


        /// <summary>
        /// User Roles
        /// </summary>
        [SwaggerIgnore]
        public List<NetAuth.Contract.DataContract.Dto.RoleDto> Roles { get; set; }

        /// <summary>
        /// Teams this user belongs to
        /// </summary>
        [SwaggerIgnore]
        public List<NetAuth.Contract.DataContract.Dto.TeamDto> Teams { get; set; } = new();

        /// <summary>
        /// Permissions granted to User
        /// </summary>
        [SwaggerIgnore]
        public List<NetAuth.Contract.DataContract.Entities.Permission> PermissionsGranted { get; set; }

        /// <summary>
        /// Permissions denied to User
        /// </summary>
        [SwaggerIgnore]
        public List<NetAuth.Contract.DataContract.Entities.Permission> PermissionsDenied { get; set; }

        /// <summary>
        /// User permissions = Role permissions + granted permissions
        /// </summary>
        [SwaggerIgnore]
        public List<NetAuth.Contract.DataContract.Entities.Permission> UserPermissions { get; set; }

        [SwaggerIgnore]
        public List<Query.UiPermission.UserUiPermission.UserUiPermissionDto> UserUiPermissions { get; set; }

        public string AccessLevel { get; set; }

        //Mapping UsersDto with Users entity
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NetAuth.Contract.DataContract.Dto.UserDto, UserDto>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(e => e.Id));
        }
    }
}
