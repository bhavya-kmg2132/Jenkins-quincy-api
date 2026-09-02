using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Application.AllUsers.Queries.AllUser.GetUsersQuery;
using Application.AllUsers.Queries.AllUser.GetUsersQueryAsync;
using Application.Users.Commands.ActivateOrInActivateUser;
using Application.Users.Commands.AddRoles;
using Application.Users.Commands.CreateUser;
using Application.Users.Commands.ResetUserCache;
using Application.Users.Commands.RolePermissions;
using Application.Users.Commands.UiPermission.AddUiPermission;
using Application.Users.Commands.UiPermission.RoleUiPermissions.AddUiPermissionsForRole;
using Application.Users.Commands.Teams.AddTeam;
using Application.Users.Commands.Teams.AddTeamMembers;
using Application.Users.Commands.Teams.RemoveTeamMember;
using Application.Users.Queries.Team;
using Application.Users.Queries.Team.GetTeamsQuery;
using Application.Users.Queries.Team.GetTeamByIdQuery;
using Application.Users.Queries.Team.GetTeamsByUserIdQuery;
using Application.Users.Queries.Team.GetTeamMembersByTeamIdQuery;
using Application.Users.Commands.UiPermission.UpdateUiPermission;
using Application.Users.Commands.UpdateUser;
using Application.Users.Commands.UserPermissions.AddPermissionRequest;
using Application.Users.Commands.UserPermissions.AddPermissionsDeniedForUser;
using Application.Users.Queries;
using Application.Users.Queries.AuthReferenceLookup;
using Application.Users.Queries.AuthReferenceLookup.GetAuthReferenceLookupQuery;
using Application.Users.Queries.GetUsersByStatusQuery;
using Application.Users.Queries.Permission;
using Application.Users.Queries.Permission.GetActiveInactivePermissionQuery;
using Application.Users.Queries.Permission.GetPermissionsQuery;
using Application.Users.Queries.Permission.GetPermissionsQueryAsync;
using Application.Users.Queries.Role.GetPermissionsByRoleIdQuery;
//using Application.Users.Command.UpdateUserProfile;
using Application.Users.Queries.Role.GetRoleQuery;
using Application.Users.Queries.Role.RoleUiPermission.GetUiPermissionsByRoleIdQuery;
using Application.Users.Queries.UiPermission;
using Application.Users.Queries.UiPermission.GetUiPermissionsQuery;
using Application.Users.Queries.UiPermission.RoleUiPermission;
using Application.Users.Queries.UiPermission.UserUiPermission;
using Application.Users.Queries.User;
using Application.Users.Queries.UserActivity;
using Application.Users.Queries.UserActivity.GetUserActivitiesByUserIdsQuery;
using Application.Users.Queries.UserActivity.GetUserActivityQuery;
using Application.Users.Queries.UserUiPermision.GetUserUiPermissionsByUserIdQuery;
using Microsoft.AspNetCore.Mvc;
using Application.Users.Commands.UserPermissions.UpdatePermissionRequest;

namespace Api.Controllers
{
    /// <summary>
    /// Controller class handles incoming HTTP requests and send response back to the caller.
    /// 1. Template for Dapper 
    /// </summary>

    //AllowAnonymous :negates the Authorize Attribute and allows anonymous access.
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class NetAuthController : ApiControllerBase
    {
        /// <summary>
        /// Resets User Cache
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <returns>ActionResult</returns>
        [HttpPut("[action]")]
        public async Task<ActionResult> ResetUserObjectCache([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                             [FromHeader(Name = "X-Request-Id")] string requestId,
                                                             [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                             [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                             [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            await Mediator.Send(new ResetUserCacheRequest { });

            return NoContent();
        }

        /// <summary>
        /// GetPermissions
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <returns>PermissionListVm</returns>
        [HttpGet("Permissions")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public Task<PermissionListVm> GetPermissions([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                     [FromHeader(Name = "X-Request-Id")] string requestId,
                                                     [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                     [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                     [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // mediator's send method will call the GetPermissionsQuery for reading the Users's list
            return Mediator.Send(new GetPermissionsQuery { });
        }

        /// <summary>
        /// GetPermissionsAsync
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <returns>PermissionListVm</returns>
        [HttpGet("PermissionsAsync")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public Task<PermissionListVm> GetPermissionsAsync([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                     [FromHeader(Name = "X-Request-Id")] string requestId,
                                                     [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                     [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                     [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // mediator's send method will call the GetPermissionsQueryAsync for reading the Users's list
            return Mediator.Send(new GetPermissionsQueryAsync { });
        }

        /// <summary>
        /// GetActiveInactivePermission
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <returns>PermissionListVm</returns>
        [HttpGet("GetActiveInactivePermissions")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public Task<PermissionListVm> GetActiveInactivePermission([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                     [FromHeader(Name = "X-Request-Id")] string requestId,
                                                     [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                     [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                     [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // mediator's send method will call the GetActiveInactivePermissionQuery for reading all Permissions, active and inactive
            return Mediator.Send(new GetActiveInactivePermissionQuery { });
        }

        /// <summary>
        /// GetRoles
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <returns>RoleListVm</returns>
        [HttpGet("Roles")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<RoleListVm>> GetRoles([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                             [FromHeader(Name = "X-Request-Id")] string requestId,
                                                             [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                             [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                             [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // mediator's send method will call the GetRolesQuery for reading the Users's list
            return await Mediator.Send(new GetRolesQuery());
        }

        /// <summary>
        /// GetUsers
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <returns>UserListVm</returns>
        [HttpGet("GetUsers")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public Task<UserListVm> GetUsers([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                         [FromHeader(Name = "X-Request-Id")] string requestId,
                                         [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                         [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                         [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // mediator's send method will call the GetUsersQuery for reading the Users's list
            return Mediator.Send(new GetUsersQuery { });
        }

        /// <summary>
        /// GetUsersAsync
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <returns>UserListVm</returns>
        [HttpGet("GetUsersAsync")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<UserListVm>> GetUsersAsync([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                  [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                  [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                  [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                  [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // mediator's send method will call the GetUsersQueryAsync for reading the Users's list
            return await Mediator.Send(new GetUsersQueryAsync { });
        }

        /// <summary>
        /// GetUserVmByUserName
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="userName"></param>
        /// <returns>UserVm</returns>
        [HttpGet("GetUserVmByUserName")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(UserVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<UserVm>> GetUserVmByUserName([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                    [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                    [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                    [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                    [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                    string userName)
        {
            // mediator's send method will call the GetUserVmByUserNameQuery for reading the User
            return await Mediator.Send(new GetUserVmByUserNameQuery { UserName = userName });
        }


        /// <summary>
        /// GetUserByRoleId
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="roleId"></param>
        /// <returns>UserListVm</returns>
        [HttpGet("GetUserByRoleId")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(UserListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<UserListVm>> GetUserByRoleId([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                    [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                    [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                    [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                    [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                    string roleId)
        {
            // mediator's send method will call the GetUserByRoleIdQuery for reading the Users's list
            return await Mediator.Send(new GetUserByRoleIdQuery { RoleId = roleId });
        }

        /// <summary>
        /// GetPermissionsByRoleId
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="roleId"></param>
        /// <returns>RoleVm</returns>
        [HttpGet("GetPermissionsByRoleId")]
        [ProducesResponseType(200, Type = typeof(PermissionListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<RoleVm>> GetPermissionsByRoleId([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                       [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                       [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                       [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                       [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                       string roleId)
        {
            // mediator's send method will call the GetPermissionsByRoleIdQuery for reading the Permissions's list
            return await Mediator.Send(new GetPermissionsByRoleId { RoleId = roleId });
        }

        /// <summary>
        /// AddPermission
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>int</returns>
        [HttpPost()]
        [Route("AddPermission")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> AddPermission([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                           [FromHeader(Name = "X-Request-Id")] string requestId,
                                                           [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                           [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                           [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                           AddPermissionRequest request)
        {
            // mediator's send method will call the AddPermissionsRequest
            await Mediator.Send(request);

            return (int)HttpStatusCode.OK;
        }



        /// <summary>
        /// UpdatePermission
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>int</returns>
        [HttpPost()]
        [Route("UpdatePermission")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> UpdatePermission([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                           [FromHeader(Name = "X-Request-Id")] string requestId,
                                                           [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                           [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                           [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                           UpdatePermissionRequest request)
        {
            // mediator's send method will call the UpdatePermissionsRequest
            await Mediator.Send(request);

            return (int)HttpStatusCode.OK;
        }

        /// <summary>
        /// UpdateUser
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("UpdateUser")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> UpdateUser([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                         [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                         [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                         [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                         [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                         UpdateUserRequest request)
        {
            // mediator's send method will call the UpdateUserRequest
            await Mediator.Send(request);

            return (int)HttpStatusCode.OK;
        }


        #region UiPermission 
        /// <summary>
        /// GetUiPermissions
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <returns>UiPermissionListVm</returns>
        [HttpGet("UiPermissions")]
        [ProducesResponseType(200, Type = typeof(UiPermissionListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<UiPermissionListVm>> GetUiPermissions([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                             [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                             [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                             [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                             [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // mediator's send method will call the GetUiPermissionsQuery for reading the UiPermission's list
            return await Mediator.Send(new GetUiPermissionsQuery { });
        }

        /// <summary>
        /// GetUiPermissionsByUserId
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="userId"></param>
        /// <returns>UserUiPermissionListVm</returns>
        [HttpGet("GetUiPermissionsByUserId")]
        [ProducesResponseType(200, Type = typeof(PermissionListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<UserUiPermissionListVm>> GetUiPermissionsByUserId([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                                         [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                                         [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                                         [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                                         [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                                         string userId)
        {
            // mediator's send method will call the GetUserUiPermissionsByUserIdQuery for reading the UiPermissions's list
            return await Mediator.Send(new GetUserUiPermissionsByUserIdQuery { UserId = userId });
        }

        /// <summary>
        /// Get roles uiPermissions by id
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="roleId"></param>
        /// <returns>RoleUiPermissionListVm</returns>
        [HttpGet("GetUiPermissionsByRoleId")]
        [ProducesResponseType(200, Type = typeof(PermissionListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<RoleUiPermissionListVm>> GetUiPermissionsByRoleId([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                                         [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                                         [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                                         [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                                         [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                                         string roleId)
        {
            // mediator's send method will call the GetUiPermissionsByRoleIdQuery for reading the UiPermissions's list
            return await Mediator.Send(new GetUiPermissionsByRoleIdQuery { RoleId = roleId });
        }

        /// <summary>
        /// Add UiPermissions For Role
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>int</returns>
        [HttpPost()]
        [Route("AddUiPermissionsForRole")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> AddUiPermissionsForRole([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                     [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                     [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                     [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                     [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                     AddUiPermissionsForRoleRequest request)
        {
            // mediator's send method will call the AddUiPermissionsForRoleRequest
            await Mediator.Send(request);

            return (int)HttpStatusCode.OK;
        }

        /// <summary>
        /// AddUiPermission
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>string</returns>
        [HttpPost()]
        [Route("AddUiPermission")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<string>> AddUiPermission([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                AddUiPermissionRequest request)
        {
            // mediator's send method will call the AddUiPermissionsRequest
            return await Mediator.Send(request);

        }

        /// <summary>
        /// UpdateUiPermission
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>int</returns>
        [HttpPost()]
        [Route("UpdateUiPermission")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> UpdateUiPermission([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                UpdateUiPermissionRequest request)
        {
            // mediator's send method will call the UpdateUiPermissionsRequest
            await Mediator.Send(request);

            return (int)HttpStatusCode.OK;
        }
        #endregion

        #region UserActivity
        /// <summary>
        /// GetUserActivities
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="userId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns>UserActivitiesVm</returns>
        [HttpGet()]
        [Route("UserActivities")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(UserActivitiesVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<UserActivitiesVm>> GetUserActivities([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                            [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                            [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                            [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                            [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                            string userId, int pageSize, int pageNumber)
        {
            // mediator's send method will call the GetUserActivitiesQuery
            return await Mediator.Send(new GetUserActivitiesQuery { UserId = userId, PageSize = pageSize, PageNumber = pageNumber });
        }

        /// <summary>
        /// GetUserActivitiesByUserIds
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="query"></param>
        /// <returns>UserActivitiesVm</returns>
        [HttpPost()]
        [Route("GetUserActivitiesByUserIds")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(UserActivitiesVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<UserActivitiesVm>> GetUserActivitiesByUserIds([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                                     [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                                     [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                                     [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                                     [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                                     GetUserActivitiesByUserIdsQuery query)
        {
            // mediator's send method will call the GetUserActivitiesByUserIdsQuery
            return await Mediator.Send(query);
        }

        /// <summary>
        /// AddUserActivity
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>string</returns>
        [HttpPost()]
        [Route("AddUserActivity")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<string>> AddUserActivity([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                AddUserActivityRequest request)
        {
            // mediator's send method will call the AddUserActivityRequest
            return await Mediator.Send(request);

        }

        /// <summary>
        /// Add PermissionsGranted For User
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>int</returns>
        [HttpPost()]
        [Route("AddPermissionsGrantedForUser")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> AddPermissionsGrantedForUser([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                          [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                          [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                          [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                          [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                          AddPermissionGrantedForUserRequest request)
        {
            // mediator's send method will call the AddPermissionGrantedForUserRequest
            await Mediator.Send(request);

            return (int)HttpStatusCode.OK;
        }

        /// <summary>
        /// Add PermissionsDenied For User
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>int</returns>
        [HttpPost()]
        [Route("AddPermissionsDeniedForUser")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> AddPermissionsDeniedForUser([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                         [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                         [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                         [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                         [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                         AddPermissionDeniedForUserRequest request)
        {
            // mediator's send method will call the AddPermissionDeniedForUserRequest
            await Mediator.Send(request);

            return (int)HttpStatusCode.OK;
        }

        /// <summary>
        /// Add Permissions For Role
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>int</returns>
        [HttpPost()]
        [Route("AddPermissionsForRole")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> AddPermissionsForRole([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                   [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                   [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                   [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                   [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                   AddPermissionsForRoleRequest request)
        {
            // mediator's send method will call the AddPermissionsForRoleRequest
            await Mediator.Send(request);

            return (int)HttpStatusCode.OK;
        }

        /// <summary>
        /// AddUser
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>string</returns>
        [HttpPost()]
        [Route("AddUser")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<string>> AddUser([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                        [FromHeader(Name = "X-Request-Id")] string requestId,
                                                        [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                        [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                        [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                        CreateUserRequest request)
        {
            // mediator's send method will call the CreateUserRequest
            string id = await Mediator.Send(request);

            return id;
        }



        /// <summary>
        /// AddRoles
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>int</returns>
        [HttpPost()]
        [Route("AddRoles")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> AddRoles([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                      [FromHeader(Name = "X-Request-Id")] string requestId,
                                                      [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                      [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                      [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                      AddRolesForUserRequest request)
        {
            // mediator's send method will call the AddRolesForUserRequest
            await Mediator.Send(request);

            return (int)HttpStatusCode.OK;
        }

        /// <summary>
        /// AddRoles
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>int</returns>
        [HttpPost()]
        [Route("AddRole")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> AddRoles([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                      [FromHeader(Name = "X-Request-Id")] string requestId,
                                                      [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                      [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                      [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                      AddRoleForUserRequest request)
        {
            // mediator's send method will call the AddRoleForUserRequest
            await Mediator.Send(request);

            return (int)HttpStatusCode.OK;
        }

        /// <summary>
        /// DeleteRole
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns>int</returns>
        [HttpPost()]
        [Route("DeleteRole")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> DeleteRole([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                      [FromHeader(Name = "X-Request-Id")] string requestId,
                                                      [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                      [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                      [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                      DeleteRoleForUserRequest request)
        {
            // mediator's send method will call the DeleteRoleForUserRequest
            await Mediator.Send(request);

            return (int)HttpStatusCode.OK;
        }

        /// <summary>
        /// GetAuthReferenceLookupsByTypeName
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="type"></param>
        /// <returns>AuthReferenceLookupVm</returns>
        [HttpGet()]
        [Route("GetAuthReferenceLookupsByTypeName")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(AuthReferenceLookupVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<AuthReferenceLookupVm>> GetAuthReferenceLookupsByTypeName([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                                                [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                                                [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                                                [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                                                [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                                                string type)
        {
            // mediator's send method will call the GetAuthReferenceLookupQuery
            return await Mediator.Send(new GetAuthReferenceLookupQuery { Type = type });
        }

        /// <summary>
        /// Activate Or InActivate User
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestOid"></param>
        /// <param name="requestUid"></param>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("ActivateOrInActivateUser")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> ActivateOrInActivateUser([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                         [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                         [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                         [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                         [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                         ActivateOrInActivateUserRequest request)
        {
            // mediator's send method will call the ActivateOrInActivateUserRequest
            await Mediator.Send(request);

            return (int)HttpStatusCode.OK;
        }

        [HttpGet("GetUsersByStatus")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(List<UsersDto>))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<List<UsersDto>>> GetUsersByStatus([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                 [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                 [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                 [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                 [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                 string Status = "all")
        {
            // mediator's send method will call the GetUsersByStatusQuery
            return await Mediator.Send(new GetUsersByStatusQuery { Status = Status });
        }
        #endregion

        #region Team

        [HttpGet("GetTeams")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(TeamListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<TeamListVm>> GetTeams([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                              [FromHeader(Name = "X-Request-Id")] string requestId,
                                                              [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                              [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                              [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            return await Mediator.Send(new GetTeamsQuery());
        }

        [HttpGet("GetTeamById/{teamId}")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(TeamVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<TeamVm>> GetTeamById([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                             [FromHeader(Name = "X-Request-Id")] string requestId,
                                                             [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                             [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                             [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                             string teamId)
        {
            return await Mediator.Send(new GetTeamByIdQuery { TeamId = teamId });
        }

        [HttpGet("GetTeamsByUserId/{userId}")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(TeamListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<TeamListVm>> GetTeamsByUserId([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                      [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                      [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                      [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                      [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                      string userId)
        {
            return await Mediator.Send(new GetTeamsByUserIdQuery { UserId = userId });
        }

        [HttpPost()]
        [Route("AddTeam")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<string>> AddTeam([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                         [FromHeader(Name = "X-Request-Id")] string requestId,
                                                         [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                         [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                         [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                         AddTeamRequest request)
        {
            return await Mediator.Send(request);
        }

        [HttpPost()]
        [Route("AddTeamMembers")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> AddTeamMembers([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                             [FromHeader(Name = "X-Request-Id")] string requestId,
                                                             [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                             [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                             [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                             AddTeamMembersRequest request)
        {
            await Mediator.Send(request);
            return (int)HttpStatusCode.OK;
        }

        [HttpPost()]
        [Route("RemoveTeamMember")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<int>> RemoveTeamMember([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                               [FromHeader(Name = "X-Request-Id")] string requestId,
                                                               [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                               [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                               [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                               RemoveTeamMemberRequest request)
        {
            await Mediator.Send(request);
            return (int)HttpStatusCode.OK;
        }

        [HttpGet("GetTeamMembersByTeamId/{teamId}")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(TeamMemberListVm))]
        [ProducesResponseType(400, Type = typeof(BadRequestObjectResult))]
        public async Task<ActionResult<TeamMemberListVm>> GetTeamMembersByTeamId([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                                                  [FromHeader(Name = "X-Request-Id")] string requestId,
                                                                                  [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                                                  [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                                                  [FromHeader(Name = "X-Api-Key")] string apiKey,
                                                                                  string teamId)
        {
            return await Mediator.Send(new GetTeamMembersByTeamIdQuery { TeamId = teamId });
        }

        #endregion
    }
}

