using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.NetAuth.EndPoints
{
    using static Testing;

    [TestFixture]
    public class EndpointNetAuthUserActivitiesTests : NetAuthTestBase
    {
        // ── GET /UserActivities ───────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetUserActivities_WithRealUserId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_userId))
                Assert.Inconclusive("No user ID found in DB — skipping GetUserActivities test.");

            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUserActivities);
            req.AddQueryParameter("userId", _userId);
            req.AddQueryParameter("pageSize", "10");
            req.AddQueryParameter("pageNumber", "1");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetUserActivities_WithEmptyUserId_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUserActivities);
            req.AddQueryParameter("userId", string.Empty);
            req.AddQueryParameter("pageSize", "10");
            req.AddQueryParameter("pageNumber", "1");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Ep_NetAuth_GetUserActivities_WithoutHeaders_ShouldNotReturn500()
        {
            if (string.IsNullOrEmpty(_userId))
                Assert.Inconclusive("No user ID found in DB — skipping GetUserActivities without headers test.");

            var req = BuildRequestWithoutHeaders(Method.Get, EndPointsSettings.ApiEndPoint.NetAuthGetUserActivities);
            req.AddQueryParameter("userId", _userId);
            req.AddQueryParameter("pageSize", "10");
            req.AddQueryParameter("pageNumber", "1");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }

        // ── POST /GetUserActivitiesByUserIds ──────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetUserActivitiesByUserIds_WithRealUserId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_userId))
                Assert.Inconclusive("No user ID found in DB — skipping GetUserActivitiesByUserIds test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthGetUserActivitiesByUserIds);
            req.AddJsonBody(new
            {
                UserIds = new List<string> { _userId }
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetUserActivitiesByUserIds_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthGetUserActivitiesByUserIds);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest)
                    .Or.EqualTo(HttpStatusCode.UnprocessableEntity));
        }

        // ── POST /AddUserActivity ─────────────────────────────────────────────

        [Test]
        public async Task Ep_NetAuth_AddUserActivity_WithRealUserId_ShouldReturn200()
        {
            if (string.IsNullOrEmpty(_userId))
                Assert.Inconclusive("No user ID found in DB — skipping AddUserActivity test.");

            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddUserActivity);
            req.AddJsonBody(new
            {
                UserId = _userId,
                ActivityType = "LOGIN",
                Description = "Integration test activity",
                IpAddress = "127.0.0.1"
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_AddUserActivity_WithEmptyBody_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Post, EndPointsSettings.ApiEndPoint.NetAuthAddUserActivity);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest)
                    .Or.EqualTo(HttpStatusCode.UnprocessableEntity));
        }

        // ── GET /GetAuthReferenceLookupsByTypeName ────────────────────────────

        [Test]
        public async Task Ep_NetAuth_GetAuthReferenceLookupsByTypeName_WithValidType_ShouldReturn200()
        {
            var req = BuildNetAuthRequest(Method.Get,
                EndPointsSettings.ApiEndPoint.NetAuthGetAuthReferenceLookupsByTypeName);
            req.AddQueryParameter("type", "AuthType");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_NetAuth_GetAuthReferenceLookupsByTypeName_WithEmptyType_ShouldReturn200OrBadRequest()
        {
            var req = BuildNetAuthRequest(Method.Get,
                EndPointsSettings.ApiEndPoint.NetAuthGetAuthReferenceLookupsByTypeName);
            req.AddQueryParameter("type", string.Empty);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Ep_NetAuth_GetAuthReferenceLookupsByTypeName_WithoutHeaders_ShouldNotReturn500()
        {
            var req = BuildRequestWithoutHeaders(Method.Get,
                EndPointsSettings.ApiEndPoint.NetAuthGetAuthReferenceLookupsByTypeName);
            req.AddQueryParameter("type", "AuthType");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }
    }
}
