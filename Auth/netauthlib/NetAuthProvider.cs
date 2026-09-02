using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetAuth.Domain.Dto;
using NetAuth.Domain.Entities;
using NetAuth.Domain.Enums;
using NetAuth.Interfaces;
using NetAuth.Lib.Domain.Entities.Entities;


namespace netauthlib
{
    internal class NetAuthProvider : INetAuthProvider
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NetAuthProvider> _logger;
        private readonly IUserDataAccess _userDataAccess;
        private readonly IUiPermissionDataAccess _uiPermissionDataAccess;
        private readonly IIdentityManager _identityManager;
        private string _clientId = string.Empty;
        private string _clientSecretValue = string.Empty;
        private string _tenantId = string.Empty;
        private string _exposedApiScope = string.Empty;

        public NetAuthProvider(IConfiguration configuration, ILogger<NetAuthProvider> logger, IIdentityManager identityManager, IUserDataAccess userDataAccess, IUiPermissionDataAccess uiPermissionDataAccess)
        {
            _configuration = configuration;
            _logger = logger;
            _identityManager = identityManager;
            _userDataAccess = userDataAccess;
            _uiPermissionDataAccess = uiPermissionDataAccess;
            this._tenantId = this._configuration["NetAuth.AzureAd:TenantId"];
            this._clientId = this._configuration["NetAuth.AzureAd:ClientId"];
            this._exposedApiScope = this._configuration["NetAuth.AzureAd:ExposedApiScope"];
            this._clientSecretValue = this._configuration["NetAuth.AzureAd:SecretValue"];
        }

        #region Private mapping helpers

        private static NetAuth.Contract.DataContract.Entities.Permission MapPermission(Permission p)
        {
            if (p == null) return null;
            return new NetAuth.Contract.DataContract.Entities.Permission
            {
                PermissionId          = p.PermissionId,
                PermissionValue       = p.PermissionValue,
                PermissionDisplayName = p.PermissionDisplayName,
                PermissionSetId       = p.PermissionSetId,
                PermissionSetName     = p.PermissionSetName,
                PermissionType        = p.PermissionType,
                ModuleId              = p.ModuleId,
                ModuleName            = p.ModuleName,
                ApiName               = p.ApiName,
                ActionPermissionEndPoint = p.ActionPermissionEndPoint,
                IsActive              = p.IsActive
            };
        }

        private static NetAuth.Contract.DataContract.Entities.UiPermission MapUiPermission(UiPermission p)
        {
            if (p == null) return null;
            return new NetAuth.Contract.DataContract.Entities.UiPermission
            {
                PermissionId          = p.PermissionId,
                PermissionValue       = p.PermissionValue,
                PermissionDisplayName = p.PermissionDisplayName,
                ModuleId              = p.ModuleId,
                ModuleName            = p.ModuleName,
                PermissionTypeId      = p.PermissionTypeId,
                PermissionTypeName    = p.PermissionTypeName,
                PermissionParentId    = p.PermissionParentId,
                PermissionParentName  = p.PermissionParentName,
                IsActive              = p.IsActive
            };
        }

        private static NetAuth.Contract.DataContract.Dto.RoleDto MapRoleDto(RoleDto r)
        {
            if (r == null) return null;
            return new NetAuth.Contract.DataContract.Dto.RoleDto
            {
                Id             = r.Id,
                RoleName       = r.RoleName,
                RoleValue      = r.RoleValue,
                RolePermissions = r.RolePermissions?.Select(MapPermission).ToList() ?? new()
            };
        }

        private static NetAuth.Contract.DataContract.Dto.TeamDto MapTeamDto(TeamDto t)
        {
            if (t == null) return null;
            return new NetAuth.Contract.DataContract.Dto.TeamDto
            {
                Id            = t.Id,
                TeamName      = t.TeamName,
                TeamShortName = t.TeamShortName,
                Description   = t.Description,
                TeamOwnerId   = t.TeamOwnerId,
                TeamCaptainId = t.TeamCaptainId,
                MemberIds     = t.MemberIds ?? new()
            };
        }

        private static NetAuth.Contract.DataContract.Dto.UserDto MapUserDto(UserDto u)
        {
            if (u == null) return null;
            return new NetAuth.Contract.DataContract.Dto.UserDto
            {
                Id                 = u.Id,
                EmpId              = u.EmpId,
                EmpType            = u.EmpType,
                UserRoleId         = u.UserRoleId,
                auth_type          = u.auth_type,
                UserName           = u.UserName,
                Mobile             = u.Mobile,
                Email              = u.Email,
                Position           = u.Position,
                BusinessUnit       = u.BusinessUnit,
                oid                = u.oid,
                given_name         = u.given_name,
                family_name        = u.family_name,
                preferred_username = u.preferred_username,
                FirstName          = u.FirstName,
                LastName           = u.LastName,
                SecondaryEmail     = u.SecondaryEmail,
                PhoneNumber        = u.PhoneNumber,
                Extension          = u.Extension,
                display_name       = u.display_name,
                ManagerId          = u.ManagerId,
                Designation        = u.Designation,
                Department         = u.Department,
                Location           = u.Location,
                Organization       = u.Organization,
                AccessLevel        = u.AccessLevel,
                Roles              = u.Roles?.Select(MapRoleDto).ToList() ?? new(),
                Teams              = u.Teams?.Select(MapTeamDto).ToList() ?? new(),
                PermissionsGranted = u.PermissionsGranted?.Select(MapPermission).ToList() ?? new(),
                PermissionsDenied  = u.PermissionsDenied?.Select(MapPermission).ToList() ?? new(),
                UserPermissions    = u.UserPermissions?.Select(MapPermission).ToList() ?? new(),
                UserUiPermissions  = u.UserUiPermissions?.Select(MapUserUiPermissionDto).ToList() ?? new()
            };
        }

        private static NetAuth.Contract.DataContract.Entities.IdentityUser MapIdentityUserDto(IdentityUser u)
        {
            if (u == null) return null;
            return new NetAuth.Contract.DataContract.Entities.IdentityUser
            {
                CacheTimeStamp     = u.CacheTimeStamp,
                UserId             = u.UserId,
                UserName           = u.UserName,
                oid                = u.oid,
                Mobile             = u.Mobile,
                Email              = u.Email,
                Position           = u.Position,
                BusinessUnit       = u.BusinessUnit,
                given_name         = u.given_name,
                family_name        = u.family_name,
                preferred_username = u.preferred_username,
                FirstName          = u.FirstName,
                LastName           = u.LastName,
                SecondaryEmail     = u.SecondaryEmail,
                auth_type          = u.auth_type,
                PhoneNumber        = u.PhoneNumber,
                Extension          = u.Extension,
                display_name       = u.display_name,
                ManagerId          = u.ManagerId,
                LastLoginTime      = u.LastLoginTime,
                TimeZone           = u.TimeZone,
                AccessLevel        = u.AccessLevel,
                UserAccessLevelName = u.UserAccessLevelName,
                ProducerLevel      = u.ProducerLevel,
                ProducerLevelDate  = u.ProducerLevelDate,
                RegionCode         = u.RegionCode,
                IsActive           = u.IsActive,
                IsDeleted          = u.IsDeleted,
                UserRoles          = u.UserRoles?.Select(r => new NetAuth.Contract.DataContract.Entities.IdentityUserRole
                {
                    Id        = r.Id,
                    UserId    = r.UserId,
                    RoleName  = r.RoleName,
                    RoleValue = r.RoleValue
                }).ToList() ?? new(),
                UserPermissions    = u.UserPermissions?.Select(MapPermission).ToList() ?? new()
            };
        }

        private static NetAuth.Contract.DataContract.Dto.UserUiPermissionDto MapUserUiPermissionDto(UserUiPermissionDto u)
        {
            if (u == null) return null;
            return new NetAuth.Contract.DataContract.Dto.UserUiPermissionDto
            {
                UserId       = u.UserId,
                UiPermission = MapUiPermission(u.UiPermission)
            };
        }

        private static NetAuth.Contract.DataContract.Dto.RoleUiPermissionDto MapRoleUiPermissionDto(RoleUiPermissionDto r)
        {
            if (r == null) return null;
            return new NetAuth.Contract.DataContract.Dto.RoleUiPermissionDto
            {
                RoleId       = r.RoleId,
                UiPermission = MapUiPermission(r.UiPermission)
            };
        }

        private static NetAuth.Contract.DataContract.Entities.UserActivity MapUserActivity(UserActivity a)
        {
            if (a == null) return null;
            return new NetAuth.Contract.DataContract.Entities.UserActivity
            {
                Id                   = a.Id,
                UserId               = a.UserId,
                LastLoginDateTime    = a.LastLoginDateTime,
                LastLogoutDateTime   = a.LastLogoutDateTime,
                LastActivityDateTime = a.LastActivityDateTime,
                LastActivityModule   = a.LastActivityModule,
                LastActionType       = a.LastActionType.HasValue
                                        ? Enum.Parse<NetAuth.Contract.DataContract.Enum.UserActionType>(a.LastActionType.Value.ToString())
                                        : null,
                LastActivityDetail   = a.LastActivityDetail
            };
        }

        private static NetAuth.Contract.DataContract.Dto.UserActivityDto MapUserActivityDto(UserActivityDto a)
        {
            if (a == null) return null;
            return new NetAuth.Contract.DataContract.Dto.UserActivityDto
            {
                Id       = a.UserActivityId,
                UserId               = a.UserId,
                LastLoginDateTime    = a.LastLoginDateTime,
                LastLogoutDateTime   = a.LastLogoutDateTime,
                LastActivityDateTime = a.LastActivityDateTime,
                LastActivityModule   = a.LastActivityModule,
                LastActionType       = a.LastActionType.HasValue
                                        ? Enum.Parse<NetAuth.Contract.DataContract.Enum.UserActionType>(a.LastActionType.Value.ToString())
                                        : null,
                LastActivityDetail   = a.LastActivityDetail,
                CreatedBy            = a.CreatedBy,
                CreatedDateTime      = a.CreatedDateTime,
                UpdatedBy            = a.UpdatedBy,
                UpdatedDateTime      = a.UpdatedDateTime,
                PublishEventId       = a.PublishEventId,
                EventName            = a.EventName,
                OperationDateTimeUtc = a.OperationDateTimeUtc,
                Data                 = a.Data
            };
        }

        private static NetAuth.Contract.DataContract.Entities.AuthReferenceLookup MapAuthReferenceLookup(AuthReferenceLookup a)
        {
            if (a == null) return null;
            return new NetAuth.Contract.DataContract.Entities.AuthReferenceLookup
            {
                Id          = a.Id,
                DisplayName = a.DisplayName,
                Name        = a.Name,
                Type        = a.Type
            };
        }

        private static NetAuth.Contract.DataContract.Entities.UserPasswordHash MapUserPasswordHash(UserPasswordHash h)
        {
            if (h == null) return null;
            return new NetAuth.Contract.DataContract.Entities.UserPasswordHash
            {
                UserId       = h.UserId,
                PasswordHash = h.PasswordHash
            };
        }

        private static NetAuth.Contract.DataContract.Dto.UsersDto MapUsersDto(UsersDto u)
        {
            if (u == null) return null;
            return new NetAuth.Contract.DataContract.Dto.UsersDto
            {
                userId       = u.userId,
                display_name = u.display_name,
                Email        = u.Email,
                IsActive     = u.IsActive,
                Roles        = u.Roles
            };
        }

        
        private static NetAuth.Contract.DataContract.Dto.TeamMemberDto MapTeamMemberDto(TeamMemberDto t)
        {
            if (t == null) return null;
            return new NetAuth.Contract.DataContract.Dto.TeamMemberDto
            {
                MemberId = t.MemberId,
                Email = t.Email,
                UserName = t.UserName,
                FirstName = t.FirstName,
                LastName = t.LastName,
                Mobile = t.Mobile,
                preferred_username = t.preferred_username,
                Designation = t.Designation,
                Department = t.Department,
                AccessLevel = t.AccessLevel
            };
        }

        #endregion

        public async Task<List<NetAuth.Contract.DataContract.Entities.Permission>> GetPermissionsAsync()
        {
            var items = await _identityManager.GetPermissionsAsync();
            return items?.Select(MapPermission).ToList() ?? new();
        }

        public async Task<List<NetAuth.Contract.DataContract.Entities.Permission>> GetAllPermissionsAsync()
        {
            var items = await _identityManager.GetAllPermissionsAsync();
            return items?.Select(MapPermission).ToList() ?? new();
        }

        public async Task<List<NetAuth.Contract.DataContract.Dto.RoleDto>> GetRoles()
        {
            var items = await _userDataAccess.GetRoles();
            return items?.Select(MapRoleDto).ToList() ?? new();
        }

        public async Task<List<NetAuth.Contract.DataContract.Dto.UserDto>> GetUsersAsync()
        {
            var items = await _identityManager.GetUsersAsync();
            return items?.Select(MapUserDto).ToList() ?? new();
        }

        public async Task<NetAuth.Contract.DataContract.Dto.UserDto> GetUserFromDb(string userId)
        {
            return MapUserDto(await _userDataAccess.GetUserFromDbAsync(userId));
        }

        public async Task<NetAuth.Contract.DataContract.Dto.UserDto> GetUserById(string userId)
        {
            return MapUserDto(await _userDataAccess.GetUserFromDbAsync(userId));
        }

        public async Task<NetAuth.Contract.DataContract.Entities.IdentityUser> GetIdentityUserByUserName(string userName_userId_userOid)
        {
            return MapIdentityUserDto(await _identityManager.GetIdentityUserAsync(userName_userId_userOid));
        }

        public async Task<NetAuth.Contract.DataContract.Dto.UserDto> GetUserVmByUserName(string userName)
        {
            return MapUserDto(await _userDataAccess.GetUserFromDbAsync(userName));
        }

        public async Task<NetAuth.Contract.DataContract.Dto.UserDto> GetUserByOid(string oid)
        {
            return MapUserDto(await _userDataAccess.GetUserFromDbAsync(oid));
        }

        public async Task<NetAuth.Contract.DataContract.Dto.UserDto> GetUserByUserId(string userId)
        {
            return MapUserDto(await _userDataAccess.GetUserFromDbAsync(userId));
        }

        public async Task<List<NetAuth.Contract.DataContract.Dto.UserDto>> GetUserByRoleId(string roleId)
        {
            var items = await _identityManager.GetUserByRoleIdAsync(roleId);
            return items?.Select(MapUserDto).ToList() ?? new();
        }

        public async Task<List<NetAuth.Contract.DataContract.Entities.AuthReferenceLookup>> GetAuthReferenceLookupList(string type)
        {
            var items = await _userDataAccess.GetAuthReferenceLookupList(type);
            return items?.Select(MapAuthReferenceLookup).ToList() ?? new();
        }

        #region UiPermission

        public async Task<List<NetAuth.Contract.DataContract.Dto.RoleUiPermissionDto>> GetUiPermissionsForRole(string roleId)
        {
            var items = await _uiPermissionDataAccess.GetUiPermissionsForRole(roleId);
            return items?.Select(MapRoleUiPermissionDto).ToList() ?? new();
        }

        public async Task<List<NetAuth.Contract.DataContract.Entities.UiPermission>> GetUiPermissions()
        {
            var items = await _uiPermissionDataAccess.GetUiPermissions();
            return items?.Select(MapUiPermission).ToList() ?? new();
        }

        public async Task<string> AddUiPermission(NetAuth.Contract.DataContract.Requests.AddUiPermission addUiPermission)
        {
            _logger.LogInformation("NetAuthUser.AddUiPermission - In process");

            UiPermission uiPermission = new NetAuth.Domain.Entities.UiPermission();
            uiPermission.PermissionValue       = addUiPermission.PermissionValue;
            uiPermission.PermissionDisplayName = addUiPermission.PermissionDisplayName;
            uiPermission.PermissionTypeId      = addUiPermission.PermissionTypeId;
            uiPermission.PermissionParentId    = addUiPermission.PermissionParentId;
            uiPermission.ModuleId              = addUiPermission.ModuleId;
            uiPermission.CreatedBy             = addUiPermission.CreatedBy;
            uiPermission.CreatedDateTime       = DateTime.UtcNow;

            var permissionId = await _uiPermissionDataAccess.AddUiPermission(uiPermission);

            _logger.LogInformation("NetAuthUser.AddUiPermission - Completed");
            return permissionId;
        }

        public async Task<bool> UpdateUiPermission(NetAuth.Contract.DataContract.Requests.UpdateUiPermission updateUiPermission)
        {
            _logger.LogInformation("NetAuthUser.UpdateUiPermission - In process");

            UiPermission uiPermission = new UiPermission();
            uiPermission.PermissionId          = updateUiPermission.PermissionId;
            uiPermission.PermissionDisplayName = updateUiPermission.PermissionDisplayName;
            uiPermission.IsActive              = updateUiPermission.IsActive;
            uiPermission.UpdatedBy             = updateUiPermission.UpdatedBy;
            uiPermission.UpdatedDateTime       = DateTime.UtcNow;

            await _uiPermissionDataAccess.ActivateUiPermission(uiPermission);

            _logger.LogInformation("NetAuthUser.UpdateUiPermission - Completed");
            return true;
        }

        public async Task<bool> AddUiPermissionsForRole(NetAuth.Contract.DataContract.Requests.AddUiPermissionsForRole addUiPermissionForRole)
        {
            _logger.LogInformation("NetAuthUser.AddUiPermissionsForRole - In process");

            List<RoleUiPermissionDto> roleUiPermissions = new List<RoleUiPermissionDto>();
            var roleUiPermission = new NetAuth.Domain.Dto.RoleUiPermissionDto();
            roleUiPermission.UiPermission = new NetAuth.Domain.Entities.UiPermission();
            roleUiPermission.RoleId = addUiPermissionForRole.RoleUiPermission.RoleId;
            roleUiPermission.UiPermission.PermissionId = addUiPermissionForRole.RoleUiPermission.UiPermissionId;
            roleUiPermission.CreatedBy = addUiPermissionForRole.CreatedBy;
            roleUiPermissions.Add(roleUiPermission);

            await _uiPermissionDataAccess.AddUiPermissionsForRole(roleUiPermissions);

            _logger.LogInformation("NetAuthUser.AddUiPermissionsForRole - Completed");
            return true;
        }

        #endregion

        #region UserActivity

        public async Task<string> AddUserActivity(NetAuth.Contract.DataContract.Requests.AddUserActivity addUserActivity)
        {
            _logger.LogInformation("NetAuthUser.AddUserActivity - In process");

            var entity = new NetAuth.Domain.Entities.UserActivity();
            entity.UserId               = addUserActivity.UserId;
            entity.LastLoginDateTime    = addUserActivity.LastLoginDateTime;
            entity.LastActivityDateTime = addUserActivity.LastActivityDateTime;
            entity.LastActivityModule   = addUserActivity.LastActivityModule;
            entity.LastActivityDetail   = addUserActivity.LastActivityDetail;

            if (addUserActivity.LastLoginDateTime == null)
                entity.LastLoginDateTime = DateTime.UtcNow;

            if (addUserActivity.LastActivityDateTime == null)
                entity.LastActivityDateTime = DateTime.UtcNow;

            if (addUserActivity.IsUserLogout)
                entity.LastLogoutDateTime = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(addUserActivity.LastActionType))
                entity.LastActionType = Enum.Parse<UserActionType>(addUserActivity.LastActionType);
            else
                entity.LastActionType = null;

            entity.CreatedBy       = addUserActivity.CreatedBy;
            entity.CreatedDateTime = DateTime.UtcNow;

            var userActivityId = await _userDataAccess.AddUserActivity(entity);

            _logger.LogInformation("NetAuthUser.AddUserActivity - Completed");
            return userActivityId;
        }

        public async Task<List<NetAuth.Contract.DataContract.Dto.UserActivityDto>> GetUserActivities(string userId, int pageSize, int pageNumber, string startDate, string endDate)
        {
            var items = await _userDataAccess.GetUserActivities(userId, pageSize, pageNumber, startDate, endDate);
            return items?.Select(MapUserActivityDto).ToList() ?? new();
        }

        public async Task<List<NetAuth.Contract.DataContract.Entities.UserActivity>> GetUserActivitiesByUserIds(List<string> userIds)
        {
            string stringUserIds = userIds != null ? string.Join(",", userIds) : string.Empty;
            var items = await _userDataAccess.GetUserActivitiesByUserIds(stringUserIds);
            return items?.Select(MapUserActivity).ToList() ?? new();
        }

        #endregion

        public async Task<bool> AddPermissionsGrantedForUser(string userId, List<string> permissionIds, string createdBy)
        {
            _logger.LogInformation("NetAuthUser.AddPermissionsGrantedForUser - In process");
            bool response = await _userDataAccess.AddPermissionsGrantedForUser(userId, permissionIds, createdBy);
            _logger.LogInformation("NetAuthUser.AddPermissionsGrantedForUser - Completed");
            return response;
        }

        public async Task<bool> AddPermissionsDeniedForUser(string userId, List<string> permissionIds, string createdBy)
        {
            _logger.LogInformation("NetAuthUser.AddPermissionsDeniedForUser - In process");
            bool response = await _userDataAccess.AddPermissionsDeniedForUser(userId, permissionIds, createdBy);
            _logger.LogInformation("NetAuthUser.AddPermissionsDeniedForUser - Completed");
            return response;
        }

        public async Task<bool> AddPermissionsForRole(string roleId, List<string> permissionIds, string createdBy)
        {
            _logger.LogInformation("NetAuthUser.AddPermissionsForRole - In process");
            bool response = await _userDataAccess.AddPermissionsForRole(roleId, permissionIds, createdBy);
            _logger.LogInformation("NetAuthUser.AddPermissionsForRole - Completed");
            return response;
        }

        public async Task<string> AddUser(NetAuth.Contract.DataContract.Requests.CreateUserRequest request)
        {
            _logger.LogInformation("NetAuthUser.AddUser - In process");

            NetAuth.Domain.Entities.CoreRequests.CreateUserRequest internalRequest = new();
            internalRequest.Id                 = request.Id;
            internalRequest.EmpId              = request.EmpId;
            internalRequest.EmpType            = request.EmpType;
            internalRequest.PasswordHash       = request.PasswordHash;
            internalRequest.auth_type          = request.auth_type;
            internalRequest.UserName           = request.UserName;
            internalRequest.Mobile             = request.Mobile;
            internalRequest.Email              = request.Email;
            internalRequest.Position           = request.Position;
            internalRequest.BusinessUnit       = request.BusinessUnit;
            internalRequest.oid                = request.oid;
            internalRequest.given_name         = request.given_name;
            internalRequest.family_name        = request.family_name;
            internalRequest.preferred_username = request.preferred_username;
            internalRequest.FirstName          = request.FirstName;
            internalRequest.LastName           = request.LastName;
            internalRequest.SecondaryEmail     = request.SecondaryEmail;
            internalRequest.PhoneNumber        = request.PhoneNumber;
            internalRequest.Extension          = request.Extension;
            internalRequest.display_name       = request.display_name;
            internalRequest.ManagerId          = request.ManagerId;
            internalRequest.AccessLevel        = request.AccessLevel;
            internalRequest.Designation        = request.Designation;
            internalRequest.Department         = request.Department;
            internalRequest.Location           = request.Location;
            internalRequest.Organization       = request.Organization;
            internalRequest.CreatedBy          = request.CreatedBy;

            string id = await _userDataAccess.AddUser(internalRequest);

            _logger.LogInformation("NetAuthUser.AddUser - Completed");
            return id;
        }

        public async Task<bool> AddRole(string userId, string roleId, string createdBy)
        {
            _logger.LogInformation("NetAuthUser.AddRole - In process");
            bool response = await _userDataAccess.AddRoleForUser(userId, roleId, createdBy);
            _logger.LogInformation("NetAuthUser.AddRole - Completed");
            return response;
        }

        public async Task<bool> DeleteRole(string userId, string roleId, string createdBy)
        {
            _logger.LogInformation("NetAuthUser.DeleteRole - In process");
            bool response = await _userDataAccess.DeleteRoleForUser(userId, roleId, createdBy);
            _logger.LogInformation("NetAuthUser.DeleteRole - Completed");
            return response;
        }

        public async Task<bool> AddRoles(string userId, List<string> roleIds, string createdBy)
        {
            _logger.LogInformation("NetAuthUser.AddRoles - In process");
            bool response = await _userDataAccess.AddRolesForUser(userId, roleIds, createdBy);
            _logger.LogInformation("NetAuthUser.AddRoles - Completed");
            return response;
        }

        public async Task<NetAuth.Contract.DataContract.Dto.RoleDto> GetPermissionsByRoleId(string roleId)
        {
            return MapRoleDto(await _userDataAccess.GetPermissionsForRoleAsync(roleId));
        }

        public async Task<bool> AddPermission(NetAuth.Contract.DataContract.Requests.AddPermission addPermission, string userName)
        {
            _logger.LogInformation("NetAuthUser.AddPermission - In process");

            Permission permission = new NetAuth.Domain.Entities.Permission();
            permission.PermissionValue       = addPermission.PermissionValue;
            permission.PermissionDisplayName = addPermission.PermissionDisplayName;
            permission.PermissionSetId       = addPermission.PermissionSetId;
            permission.ModuleId              = addPermission.ModuleId;
            permission.PermissionType        = addPermission.PermissionType;
            permission.IsActive              = addPermission.IsActive;

            await _userDataAccess.AddPermission(permission, userName);

            _logger.LogInformation("NetAuthUser.AddPermission - Completed");
            return true;
        }

        public async Task<bool> UpdatePermission(NetAuth.Contract.DataContract.Requests.UpdatePermission editPermission, string userName)
        {
            _logger.LogInformation("NetAuthUser.UpdatePermission - In process");

            NetAuth.Domain.Entities.UpdatePermission permission = new NetAuth.Domain.Entities.UpdatePermission();
            permission.Id                  = editPermission.Id;
            permission.PermissionValue     = editPermission.PermissionValue;
            permission.PermissionDisplayName = editPermission.PermissionDisplayName;
            permission.PermissionSetId     = editPermission.PermissionSetId;
            permission.PermissionType      = editPermission.PermissionType;
            permission.ModuleId            = editPermission.ModuleId;
            permission.IsActive            = editPermission.IsActive;
            permission.IsDeleted           = editPermission.IsDeleted;
            permission.IsApproved          = editPermission.IsApproved;
            permission.ApproverId          = editPermission.ApproverId;
            permission.ApprovedDateTime    = editPermission.ApprovedDateTime;
            permission.IsAuthorized        = editPermission.IsAuthorized;
            permission.AuthorizedById      = editPermission.AuthorizedById;
            permission.AuthorizedDateTime  = editPermission.AuthorizedDateTime;
            permission.UpdatedBy           = editPermission.UpdatedBy;

            await _userDataAccess.UpdatePermission(permission, userName);

            _logger.LogInformation("NetAuthUser.UpdatePermission - Completed");
            return true;
        }

        public async Task<int> UpdateUserPasswordHash(NetAuth.Contract.DataContract.Requests.UpdateUserPasswordHash request)
        {
            _logger.LogInformation("NetAuthUser.UpdateUserPasswordHash - In process");

            var entity = new NetAuth.Domain.Entities.UserPasswordHash();
            entity.UserId         = request.UserId;
            entity.PasswordHash   = request.PasswordHash;
            entity.UpdatedBy      = request.UpdatedBy;
            entity.UpdatedDateTime = request.UpdatedDateTime;
            entity.UpdateReason   = request.UpdateReason;

            int rowsAffected = await _userDataAccess.UpdateUserPasswordHash(entity);

            _logger.LogInformation("NetAuthUser.UpdateUserPasswordHash - Completed");
            return rowsAffected;
        }

        public async Task<NetAuth.Contract.DataContract.Entities.UserPasswordHash> GetUserPasswordHash(string userId)
        {
            return MapUserPasswordHash(await _userDataAccess.GetUserPasswordHash(userId));
        }

        public async Task<int> UpdateUser(NetAuth.Contract.DataContract.Requests.UpdateUser updateUser)
        {
            _logger.LogInformation("NetAuthUser.UpdateUser - In process");

            NetAuth.Domain.Entities.User entity = new();
            entity.Id            = updateUser.userId;
            entity.Email         = updateUser.Email;
            entity.PhoneNumber   = updateUser.PhoneNumber;
            entity.EmpId         = updateUser.EmpId;
            entity.UpdatedBy     = updateUser.UpdatedBy;
            entity.UpdatedDateTime = updateUser.UpdatedDateTime;

            int updateStatus = await _userDataAccess.UpdateUser(entity);

            _logger.LogInformation("NetAuthUser.UpdateUser - Completed");
            return updateStatus;
        }

        public async Task<int> ActivateOrInActivateUser(NetAuth.Contract.DataContract.Requests.ActivateOrInActivateUser activateOrInActivateUser)
        {
            _logger.LogInformation("NetAuthUser.ActivateOrInActivateUser - In process");
            int status = await _userDataAccess.ActivateOrInActivateUser(activateOrInActivateUser.UserId, activateOrInActivateUser.IsActive);
            _logger.LogInformation("NetAuthUser.ActivateOrInActivateUser - Completed");
            return status;
        }

        public async Task<List<NetAuth.Contract.DataContract.Dto.UsersDto>> GetUsersByStatus(string status)
        {
            var items = await _userDataAccess.GetUsersByStatus(status);
            return items?.Select(MapUsersDto).ToList() ?? new();
        }

        #region Team management

        public async Task<List<NetAuth.Contract.DataContract.Dto.TeamDto>> GetTeams()
        {
            var items = await _userDataAccess.GetTeams();
            return items?.Select(MapTeamDto).ToList() ?? new();
        }

        public async Task<NetAuth.Contract.DataContract.Dto.TeamDto> GetTeamById(string teamId)
        {
            return MapTeamDto(await _userDataAccess.GetTeamById(teamId));
        }

        public async Task<string> AddTeam(NetAuth.Contract.DataContract.Dto.TeamDto team, string createdBy)
        {
            NetAuth.Domain.Dto.TeamDto internalTeam = new NetAuth.Domain.Dto.TeamDto();
            internalTeam.Id            = team.Id;
            internalTeam.TeamName      = team.TeamName;
            internalTeam.TeamShortName = team.TeamShortName;
            internalTeam.Description   = team.Description;
            internalTeam.TeamOwnerId   = team.TeamOwnerId;
            internalTeam.TeamCaptainId = team.TeamCaptainId;
            internalTeam.MemberIds     = team.MemberIds;

            return await _userDataAccess.AddTeam(internalTeam, createdBy);
        }

        public async Task<bool> AddTeamMembers(string teamId, List<string> userIds, string createdBy)
            => await _userDataAccess.AddTeamMembers(teamId, userIds, createdBy);

        public async Task<bool> RemoveTeamMember(string teamId, string userId)
            => await _userDataAccess.RemoveTeamMember(teamId, userId);

       

        public async Task<List<NetAuth.Contract.DataContract.Dto.TeamDto>> GetTeamsByUserId(string userId)
        {
            var items = await _userDataAccess.GetTeamsByUserId(userId);
            return items?.Select(MapTeamDto).ToList() ?? new();
        }

       

        public async Task<List<NetAuth.Contract.DataContract.Dto.TeamMemberDto>> GetTeamMembersByTeamId(string teamId)
        {
            var items = await _userDataAccess.GetTeamMembersByTeamId(teamId);
            return items?.Select(MapTeamMemberDto).ToList() ?? new();
        }
        #endregion
    }
}
