using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.NetAuth.EndPoints
{
    using static Testing;

    [TestFixture]
    public class EndpointNetAuthUsersTests : NetAuthTestBase
    {
        // ── GET /GetUsers ─────────────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetUsers_WithCorrelationHeaders_ShouldReturn200()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUsers);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetUsers_WithoutCorrelationHeaders_ShouldNotReturn500()
        {
            var req = BuildRequestWithoutHeaders(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUsers);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── GET /GetUsersAsync ────────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetUsersAsync_WithCorrelationHeaders_ShouldReturn200()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUsersAsync);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetUsersAsync_WithoutCorrelationHeaders_ShouldNotReturn500()
        {
            var req = BuildRequestWithoutHeaders(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUsersAsync);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── GET /GetUserVmByUserName ──────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetUserVmByUserName_WithValidUserName_ShouldReturn200()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUserVmByUserName);
            req.AddQueryParameter("userName", AdminTestUsername);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetUserVmByUserName_WithNonExistentUser_ShouldReturn200OrNotFound()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUserVmByUserName);
            req.AddQueryParameter("userName", "nonexistent@domain.com");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.NotFound));
        }

        // ── GET /GetUserByRoleId ──────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetUserByRoleId_WithRealRoleId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_roleId))
                Assert.Inconclusive("No role found in DB — skipping GetUserByRoleId test.");

            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUserByRoleId);
            req.AddQueryParameter("roleId", _roleId);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        // ── GET /GetUsersByStatus ─────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetUsersByStatus_WithAllStatus_ShouldReturn200()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUsersByStatus);
            req.AddQueryParameter("Status", "all");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetUsersByStatus_WithActiveStatus_ShouldReturn200()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUsersByStatus);
            req.AddQueryParameter("Status", "active");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetUsersByStatus_WithDefaultStatus_ShouldReturn200()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUsersByStatus);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        // ── POST /AddUser ─────────────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_AddUser_WithValidBody_ShouldReturn200()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddUser);
            var uniqueId = Guid.NewGuid().ToString("N")[..8];
            req.AddJsonBody(new
            {
                UserName  = $"int.test.{uniqueId}@testdomain.com",
                Email     = $"int.test.{uniqueId}@testdomain.com",
                display_name = $"int.test.{uniqueId}@testdomain.com",
                oid = uniqueId,
                FirstName = "Integration",
                LastName  = "Test",
                AccessLevel = "L1",
                Mobile    = "0000000001",
                auth_type = "db"
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_AddUser_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddUser);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest)
                    .Or.EqualTo(HttpStatusCode.UnprocessableEntity));
        }

        // ── POST /UpdateUser ──────────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_UpdateUser_WithRealUserId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_userId))
                Assert.Inconclusive("No user ID found in DB — skipping UpdateUser test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthUpdateUser);
            req.AddJsonBody(new
            {
                userId      = _userId,
                Email       = AdminTestUsername,
                PhoneNumber = "0000000000",
                EmpId       = $"EMP-12"
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_UpdateUser_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthUpdateUser);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest)
                    .Or.EqualTo(HttpStatusCode.UnprocessableEntity));
        }

        // ── POST /ActivateOrInActivateUser ────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_ActivateOrInActivateUser_WithRealUserId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_userId))
                Assert.Inconclusive("No user ID found in DB — skipping ActivateOrInActivateUser test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthActivateOrInActivateUser);
            req.AddJsonBody(new
            {
                userId = _userId,
                IsActive = true
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_ActivateOrInActivateUser_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthActivateOrInActivateUser);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest)
                    .Or.EqualTo(HttpStatusCode.UnprocessableEntity));
        }

        // ── PUT /ResetUserObjectCache ─────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_ResetUserObjectCache_WithCorrelationHeaders_ShouldReturn204()
        {
            var req = BuildNetAuthRequest(Method.Put, EndPointsSettings.ApiEndPoint.NetAuthResetUserObjectCache);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}
