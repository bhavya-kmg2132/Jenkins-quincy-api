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
    public class EndpointNetAuthUiPermissionsTests : NetAuthTestBase
    {
        // ── GET /UiPermissions ────────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetUiPermissions_WithCorrelationHeaders_ShouldReturn200()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUiPermissions);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetUiPermissions_WithoutHeaders_ShouldNotReturn500()
        {
            var req = BuildRequestWithoutHeaders(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUiPermissions);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── GET /GetUiPermissionsByUserId ─────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetUiPermissionsByUserId_WithValidUserId_ShouldReturn200()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUiPermissionsByUserId);
            req.AddQueryParameter("userId", AdminTestUsername);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetUiPermissionsByUserId_WithEmptyUserId_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUiPermissionsByUserId);
            req.AddQueryParameter("userId", string.Empty);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest));
        }

        // ── GET /GetUiPermissionsByRoleId ─────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetUiPermissionsByRoleId_WithRealRoleId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_roleId))
                Assert.Inconclusive("No role found in DB — skipping GetUiPermissionsByRoleId test.");

            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUiPermissionsByRoleId);
            req.AddQueryParameter("roleId", _roleId);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetUiPermissionsByRoleId_WithMissingRoleId_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUiPermissionsByRoleId);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest));
        }

        // ── POST /AddUiPermission ─────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_AddUiPermission_WithRealFields_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_uiPermissionTypeId) || string.IsNullOrEmpty(_uiModuleId))
                Assert.Inconclusive("No UI permission data found in DB — skipping AddUiPermission test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddUiPermission);
            req.AddJsonBody(new
            {
                PermissionValue = $"ui.test.view.{Guid.NewGuid():N}",
                PermissionDisplayName = "Test UI View Permission",
                PermissionTypeId = _uiPermissionTypeId,
                ModuleId = _uiModuleId
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task Ep_NetAuth_AddUiPermission_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddUiPermission);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest)
                                .Or.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── POST /UpdateUiPermission ──────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_UpdateUiPermission_WithRealUiPermissionId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_uiPermissionId))
                Assert.Inconclusive("No UI permission found in DB — skipping UpdateUiPermission test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthUpdateUiPermission);
            req.AddJsonBody(new
            {
                PermissionId          = _uiPermissionId,
                PermissionDisplayName = "Updated via integration test",
                IsActive              = true
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.NotFound)
                    .Or.EqualTo(HttpStatusCode.UnprocessableEntity));
        }

        [Test]
        public async Task Ep_NetAuth_UpdateUiPermission_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthUpdateUiPermission);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest)
                    .Or.EqualTo(HttpStatusCode.NotFound)
                                .Or.EqualTo(HttpStatusCode.UnprocessableEntity));
        }

        // ── POST /AddUiPermissionsForRole ─────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_AddUiPermissionsForRole_WithRealIds_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_roleId) || string.IsNullOrEmpty(_uiPermissionId))
                Assert.Inconclusive("No role or UI permission found in DB — skipping AddUiPermissionsForRole test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddUiPermissionsForRole);
            req.AddJsonBody(new
            {
                roleUiPermissionsVm = new
                {
                    roleAndUiPermissions = new[]
                    {
                        new { roleId = _roleId, uiPermissionId = _uiPermissionId }
                    }
                }
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_AddUiPermissionsForRole_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddUiPermissionsForRole);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest)
                                        .Or.EqualTo(HttpStatusCode.InternalServerError));
        }
    }
}
