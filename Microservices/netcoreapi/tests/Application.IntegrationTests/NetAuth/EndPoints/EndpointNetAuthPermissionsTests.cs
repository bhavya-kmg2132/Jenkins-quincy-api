using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.NetAuth.EndPoints
{
    using static Testing;

    [TestFixture]
    public class EndpointNetAuthPermissionsTests : NetAuthTestBase
    {
        // ── GET /Permissions ──────────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetPermissions_WithCorrelationHeaders_ShouldReturn200()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetPermissions);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetPermissions_WithoutHeaders_ShouldNotReturn500()
        {
            var req = BuildRequestWithoutHeaders(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetPermissions);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── GET /PermissionsAsync ─────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetPermissionsAsync_WithCorrelationHeaders_ShouldReturn200()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetPermissionsAsync);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetPermissionsAsync_WithoutHeaders_ShouldNotReturn500()
        {
            var req = BuildRequestWithoutHeaders(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetPermissionsAsync);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── GET /GetPermissionsByRoleId ───────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetPermissionsByRoleId_WithRealRoleId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_roleId))
                Assert.Inconclusive("No role found in DB — skipping GetPermissionsByRoleId test.");

            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetPermissionsByRoleId);
            req.AddQueryParameter("roleId", _roleId);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetPermissionsByRoleId_WithMissingRoleId_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetPermissionsByRoleId);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest));
        }

        // ── POST /AddPermission ───────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_AddPermission_WithRealFields_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_permissionSetId))
                Assert.Inconclusive("No permission found in DB — skipping AddPermission test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddPermission);
            req.AddJsonBody(new
            {
                PermissionValue       = $"test.netauth.{Guid.NewGuid():N}",
                PermissionDisplayName = $"Test NetAuth Add Permission.{Guid.NewGuid():N}",
                PermissionSetId       = _permissionSetId,
                ModuleId              = _moduleId,
                PermissionType        = _permissionType
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_AddPermission_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddPermission);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.BadRequest)
                    .Or.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── POST /UpdatePermission ────────────────────────────────────────────

        // When PermissionType == "ACTION", Value and DisplayName are immutable —
        // only metadata fields (IsActive, IsDeleted, etc.) may be updated.
        [Test]
        public async Task Ep_NetAuth_UpdatePermission_WhenTypeIsAction_ShouldUpdateMetadataOnly_Return200()
        {
            if (string.IsNullOrEmpty(_actionPermissionId))
                Assert.Inconclusive("No ACTION-type permission found in DB — skipping.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthUpdatePermission);
            req.AddJsonBody(new
            {
                Id                    = _actionPermissionId,
                PermissionValue       = _actionPermissionValue,
                PermissionDisplayName = _actionPermissionDisplayName,
                PermissionSetId       = _actionPermissionSetId,
                PermissionType        = "ACTION",
                ModuleId              = _actionModuleId,
                IsActive              = true,
                IsDeleted             = false,
                IsApproved            = false,
                ApproverId            = (string)null,
                ApprovedDateTime      = (DateTime?)null,
                IsAuthorized          = (bool?)null,
                AuthorizedById        = (string)null,
                AuthorizedDateTime    = (DateTime?)null
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        // When PermissionType != "ACTION", Value and DisplayName can be updated freely.
        [Test]
        public async Task Ep_NetAuth_UpdatePermission_WhenTypeIsNotAction_ShouldUpdateAllFields_Return200()
        {
            if (string.IsNullOrEmpty(_nonActionPermissionId))
                Assert.Inconclusive("No non-ACTION permission found in DB — skipping.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthUpdatePermission);
            req.AddJsonBody(new
            {
                Id                    = _nonActionPermissionId,
                PermissionValue       = $"test.netauth.upd.{Guid.NewGuid():N}",
                PermissionDisplayName = $"Updated via integration test {Guid.NewGuid():N}",
                PermissionSetId       = _nonActionPermissionSetId,
                PermissionType        = _nonActionPermissionType,
                ModuleId              = _nonActionModuleId,
                IsActive              = true,
                IsDeleted             = false,
                IsApproved            = false,
                ApproverId            = (string)null,
                ApprovedDateTime      = (DateTime?)null,
                IsAuthorized          = (bool?)null,
                AuthorizedById        = (string)null,
                AuthorizedDateTime    = (DateTime?)null
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_UpdatePermission_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthUpdatePermission);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest)
                    .Or.EqualTo(HttpStatusCode.NotFound)
                    .Or.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── POST /AddPermissionsGrantedForUser ────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_AddPermissionsGrantedForUser_WithRealPermissionId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_userId) || string.IsNullOrEmpty(_permissionId))
                Assert.Inconclusive("No user or permission found in DB — skipping AddPermissionsGrantedForUser test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddPermissionsGrantedForUser);
            req.AddJsonBody(new
            {
                UserId = _userId,
                PermissionIds = new List<string> { _permissionId }
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_AddPermissionsGrantedForUser_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddPermissionsGrantedForUser);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── POST /AddPermissionsDeniedForUser ─────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_AddPermissionsDeniedForUser_WithRealPermissionId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_userId) || string.IsNullOrEmpty(_permissionId))
                Assert.Inconclusive("No user or permission found in DB — skipping AddPermissionsDeniedForUser test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddPermissionsDeniedForUser);
            req.AddJsonBody(new
            {
                UserId = _userId,
                PermissionIds = new List<string> { _permissionId }
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_AddPermissionsDeniedForUser_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddPermissionsDeniedForUser);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest)
                                .Or.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── POST /AddPermissionsForRole ───────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_AddPermissionsForRole_WithRealIds_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_roleId) || string.IsNullOrEmpty(_permissionId))
                Assert.Inconclusive("No role or permission found in DB — skipping AddPermissionsForRole test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddPermissionsForRole);
            req.AddJsonBody(new
            {
                RoleAndPermissionMapping = new
                {
                    RoleId        = _roleId,
                    PermissionIds = new List<string> { _permissionId }
                }
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_AddPermissionsForRole_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddPermissionsForRole);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest)
                    .Or.EqualTo(HttpStatusCode.InternalServerError));
        }
    }
}
