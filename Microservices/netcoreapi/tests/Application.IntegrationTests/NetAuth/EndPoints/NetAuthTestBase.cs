using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.IntegrationTests.Auth.EndPoints;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.NetAuth.EndPoints
{
    using static Testing;

    public abstract class NetAuthTestBase : AuthTestBase
    {
        // ── Cached real-data from the DB (fetched once per test run) ─────────

        protected static string _permissionId;
        protected static string _permissionSetId;
        protected static string _permissionType;
        protected static string _moduleId;

        protected static string _roleId;
        protected static string _roleName;

        protected static string _teamId;

        protected static string _userId;
        protected static string _level1UserId;

        // ACTION-type permission (Value/DisplayName must be passed as-is on update)
        protected static string _actionPermissionId;
        protected static string _actionPermissionSetId;
        protected static string _actionModuleId;
        protected static string _actionPermissionValue;
        protected static string _actionPermissionDisplayName;

        // Non-ACTION permission (Value/DisplayName can be updated)
        protected static string _nonActionPermissionId;
        protected static string _nonActionPermissionSetId;
        protected static string _nonActionModuleId;
        protected static string _nonActionPermissionType;

        protected static string _uiPermissionId;
        protected static string _uiPermissionTypeId;
        protected static string _uiModuleId;

        private static bool _dataFetched;

        // ── Inline DTOs ───────────────────────────────────────────────────────

        private sealed class PermissionListResponse
        {
            [JsonPropertyName("permissionList")]
            public List<PermissionEntry> PermissionList { get; set; }
        }

        private sealed class PermissionEntry
        {
            [JsonPropertyName("permissionId")]          public string PermissionId          { get; set; }
            [JsonPropertyName("permissionSetId")]       public string PermissionSetId       { get; set; }
            [JsonPropertyName("permissionType")]        public string PermissionType        { get; set; }
            [JsonPropertyName("moduleId")]              public string ModuleId              { get; set; }
            [JsonPropertyName("permissionValue")]       public string PermissionValue       { get; set; }
            [JsonPropertyName("permissionDisplayName")] public string PermissionDisplayName { get; set; }
        }

        private sealed class RoleListResponse
        {
            [JsonPropertyName("rolesList")]
            public List<RoleEntry> RolesList { get; set; }
        }

        private sealed class RoleEntry
        {
            [JsonPropertyName("id")]       public string Id       { get; set; }
            [JsonPropertyName("roleName")] public string RoleName { get; set; }
        }

        private sealed class TeamListResponse
        {
            [JsonPropertyName("teams")]
            public List<TeamEntry> Teams { get; set; }
        }

        private sealed class TeamEntry
        {
            [JsonPropertyName("id")]       public string Id       { get; set; }
            [JsonPropertyName("teamName")] public string TeamName { get; set; }
        }

        private sealed class UserVmResponse
        {
            [JsonPropertyName("user")]
            public UserEntry User { get; set; }
        }

        private sealed class UserEntry
        {
            [JsonPropertyName("userId")]
            public string UserId { get; set; }
        }

        private sealed class UiPermissionListResponse
        {
            [JsonPropertyName("uiPermissionList")]
            public List<UiPermissionEntry> UiPermissionList { get; set; }
        }

        private sealed class UiPermissionEntry
        {
            [JsonPropertyName("permissionId")]     public string PermissionId     { get; set; }
            [JsonPropertyName("permissionTypeId")] public string PermissionTypeId { get; set; }
            [JsonPropertyName("moduleId")]         public string ModuleId         { get; set; }
        }

        // ── SetUp: fetch real data once per test run ──────────────────────────

        [SetUp]
        public async Task NetAuthSetUp()
        {
            if (_dataFetched) return;

            // Permissions — scan the full list once to populate general, ACTION, and non-ACTION slots
            var permReq = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetPermissionsAsync);
            var permResp = await Client.ExecuteAsync(permReq);
            if (permResp.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(permResp.Content))
            {
                var vm = JsonSerializer.Deserialize<PermissionListResponse>(permResp.Content, JsonOpts);
                var list = vm?.PermissionList ?? new List<PermissionEntry>();

                // General — first entry that has all lookup fields populated
                var first = list.FirstOrDefault(p =>
                    !string.IsNullOrEmpty(p.PermissionSetId) && !string.IsNullOrEmpty(p.ModuleId))
                    ?? list.FirstOrDefault();
                if (first != null)
                {
                    _permissionId    = first.PermissionId;
                    _permissionSetId = first.PermissionSetId;
                    _permissionType  = first.PermissionType ?? "ACTION";
                    _moduleId        = first.ModuleId;
                }

                // ACTION-type permission
                var action = list.FirstOrDefault(p =>
                    string.Equals(p.PermissionType, "ACTION", StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrEmpty(p.PermissionSetId));
                if (action != null)
                {
                    _actionPermissionId          = action.PermissionId;
                    _actionPermissionSetId       = action.PermissionSetId;
                    _actionModuleId              = action.ModuleId;
                    _actionPermissionValue       = action.PermissionValue;
                    _actionPermissionDisplayName = action.PermissionDisplayName;
                }

                // Non-ACTION permission
                var nonAction = list.FirstOrDefault(p =>
                    !string.Equals(p.PermissionType, "ACTION", StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrEmpty(p.PermissionSetId));
                if (nonAction != null)
                {
                    _nonActionPermissionId    = nonAction.PermissionId;
                    _nonActionPermissionSetId = nonAction.PermissionSetId;
                    _nonActionModuleId        = nonAction.ModuleId;
                    _nonActionPermissionType  = nonAction.PermissionType;
                }
            }

            // Roles
            var roleReq = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetRoles);
            var roleResp = await Client.ExecuteAsync(roleReq);
            if (roleResp.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(roleResp.Content))
            {
                var vm = JsonSerializer.Deserialize<RoleListResponse>(roleResp.Content, JsonOpts);
                var first = vm?.RolesList?.FirstOrDefault();
                if (first != null) { _roleId = first.Id; _roleName = first.RoleName; }
            }

            // Admin user ID
            var userReq = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUserVmByUserName);
            userReq.AddQueryParameter("userName", AdminTestUsername);
            var userResp = await Client.ExecuteAsync(userReq);
            if (userResp.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(userResp.Content))
            {
                var vm = JsonSerializer.Deserialize<UserVmResponse>(userResp.Content, JsonOpts);
                if (vm?.User != null)
                    _userId = vm.User.UserId;
            }

            // Level1 user ID (used as a team member distinct from the team owner)
            var level1Req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUserVmByUserName);
            level1Req.AddQueryParameter("userName", Level1TestUsername);
            var level1Resp = await Client.ExecuteAsync(level1Req);
            if (level1Resp.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(level1Resp.Content))
            {
                var vm = JsonSerializer.Deserialize<UserVmResponse>(level1Resp.Content, JsonOpts);
                if (vm?.User != null)
                    _level1UserId = vm.User.UserId;
            }

            // Teams
            var teamReq = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetTeams);
            var teamResp = await Client.ExecuteAsync(teamReq);
            if (teamResp.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(teamResp.Content))
            {
                var vm = JsonSerializer.Deserialize<TeamListResponse>(teamResp.Content, JsonOpts);
                var first = vm?.Teams?.FirstOrDefault();
                if (first != null) { _teamId = first.Id; }
            }

            // UI Permissions
            var uiReq = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUiPermissions);
            var uiResp = await Client.ExecuteAsync(uiReq);
            if (uiResp.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(uiResp.Content))
            {
                var vm = JsonSerializer.Deserialize<UiPermissionListResponse>(uiResp.Content, JsonOpts);
                var first = vm?.UiPermissionList?.FirstOrDefault(p =>
                    !string.IsNullOrEmpty(p.PermissionTypeId) && !string.IsNullOrEmpty(p.ModuleId))
                    ?? vm?.UiPermissionList?.FirstOrDefault();
                if (first != null)
                {
                    _uiPermissionId     = first.PermissionId;
                    _uiPermissionTypeId = first.PermissionTypeId;
                    _uiModuleId         = first.ModuleId;
                }
            }

            _dataFetched = true;

            TestContext.WriteLine(
                $"[NetAuthSetUp] userId={_userId}, level1UserId={_level1UserId}, permissionId={_permissionId}, " +
                $"permissionSetId={_permissionSetId}, permissionType={_permissionType}, moduleId={_moduleId}, " +
                $"roleId={_roleId}, teamId={_teamId}, uiPermissionId={_uiPermissionId}");
        }

        // ── Request helpers ───────────────────────────────────────────────────

        protected RestRequest BuildNetAuthRequest(Method method, string endpointPath)
            => BuildRequest(method, endpointPath, withCorrelationHeaders: true);

        protected RestRequest BuildRequestWithoutHeaders(Method method, string endpointPath)
            => BuildRequest(method, endpointPath, withCorrelationHeaders: false);
    }
}
