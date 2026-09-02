using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Common;
using NetAuth.Contract.DataContract.Dto;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.Auth.EndPoints
{
    using static Testing;

    [TestFixture]
    public class EndpointAuthRefreshTokenTests : AuthTestBase
    {
        [TearDown]
        public async Task RefreshTokenTearDown()
        {
            await CleanUpUserActivityByUsernameAsync(AdminTestUsername);
        }

        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public async Task Ep_RefreshToken_WithValidToken_ShouldReturnNewTokenModel()
        {
            // Step 1 — get a real refresh token by logging in with the shared user
            var tokenModel = await LoginAsync(AdminTestUsername, AdminTestPassword);
            Assert.That(tokenModel?.RefreshToken, Is.Not.Null.And.Not.Empty,
                "Login must succeed before refresh token test.");

            // Step 2 — exchange refresh token for new tokens
            var req = BuildRequest(Method.Post, EndPointsSettings.ApiEndPoint.AuthRefreshToken,withCorrelationHeaders:true);
            req.AddJsonBody(new RefreshRevokeRequest { RefreshToken = tokenModel.RefreshToken });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var refreshed = JsonSerializer.Deserialize<TokenModel>(response.Content, JsonOpts);
            Assert.That(refreshed?.AccessToken, Is.Not.Null.And.Not.Empty);
            Assert.That(refreshed?.RefreshToken, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task Ep_RefreshToken_WithInvalidToken_ShouldReturnUnauthorized()
        {
            var req = BuildRequest(Method.Post, EndPointsSettings.ApiEndPoint.AuthRefreshToken);
            req.AddJsonBody(new RefreshRevokeRequest { RefreshToken = System.Guid.NewGuid().ToString() });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Ep_RefreshToken_WithNullBody_ShouldReturnBadRequest()
        {
            var req = BuildRequest(Method.Post, EndPointsSettings.ApiEndPoint.AuthRefreshToken);
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, (Is.EqualTo(HttpStatusCode.BadRequest).Or.EqualTo(HttpStatusCode.Unauthorized)));
        }
    }
}
