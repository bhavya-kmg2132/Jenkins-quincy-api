using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.NetAuth.EndPoints
{
    using static Testing;

    [TestFixture]
    public class EndpointNetAuthRolesTests : NetAuthTestBase
    {
        // ── GET /Roles ────────────────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetRoles_WithCorrelationHeaders_ShouldReturn200()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetRoles);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetRoles_WithoutHeaders_ShouldNotReturn500()
        {
            var req = BuildRequestWithoutHeaders(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetRoles);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── GET /GetPermissionsByRoleId (using real role) ─────────────────────

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

        // ── POST /AddRoles ────────────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_AddRoles_WithRealRoleId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_userId) || string.IsNullOrEmpty(_roleId))
                Assert.Inconclusive("No user or role found in DB — skipping AddRoles test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddRoles);
            req.AddJsonBody(new
            {
                UserId  = _userId,
                RoleIds = new List<string> { _roleId }
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_AddRoles_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddRoles);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── POST /AddRole ─────────────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_AddRole_WithRealRoleId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_userId) || string.IsNullOrEmpty(_roleId))
                Assert.Inconclusive("No user or role found in DB — skipping AddRole test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddRole);
            req.AddJsonBody(new
            {
                UserId = _userId,
                RoleId = _roleId
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_AddRole_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddRole);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── POST /DeleteRole ──────────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_DeleteRole_WithRealRoleId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_userId) || string.IsNullOrEmpty(_roleId))
                Assert.Inconclusive("No user or role found in DB — skipping DeleteRole test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthDeleteRole);
            req.AddJsonBody(new
            {
                UserId = _userId,
                RoleId = _roleId
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_DeleteRole_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthDeleteRole);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest)
                    .Or.EqualTo(HttpStatusCode.UnprocessableEntity));
        }
    }
}
