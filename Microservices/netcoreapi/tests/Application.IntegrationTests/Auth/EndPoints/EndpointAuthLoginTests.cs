using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Entities;
using NetAuth.Contract.DataContract.Dto;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.Auth.EndPoints
{
    using static Testing;

    [TestFixture]
    public class EndpointAuthLoginTests : AuthTestBase
    {
        [TearDown]
        public async Task LoginTearDown()
        {
            await CleanUpUserActivityByUsernameAsync(AdminTestUsername);
        }

        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public async Task Ep_Login_WithValidCredentials_ShouldReturnAccessAndRefreshTokens()
        {
            var req = BuildRequest(Method.Post, EndPointsSettings.ApiEndPoint.AuthLogin, withCorrelationHeaders: true);
            req.AddJsonBody(new LoginRequest { Username = AdminTestUsername, Password = AdminTestPassword });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var token = JsonSerializer.Deserialize<TokenModel>(response.Content, JsonOpts);
            Assert.That(token?.AccessToken, Is.Not.Null.And.Not.Empty);
            Assert.That(token?.RefreshToken, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task Ep_Login_WithInvalidPassword_ShouldReturnUnauthorized()
        {
            var req = BuildRequest(Method.Post, EndPointsSettings.ApiEndPoint.AuthLogin, withCorrelationHeaders: true);
            req.AddJsonBody(new LoginRequest { Username = AdminTestUsername, Password = "WrongPassword123!" });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Ep_Login_WithNonExistentUsername_ShouldReturnUnauthorized()
        {
            var req = BuildRequest(Method.Post, EndPointsSettings.ApiEndPoint.AuthLogin, withCorrelationHeaders: true);
            req.AddJsonBody(new LoginRequest { Username = "nonexistent@domain.com", Password = "AnyPassword123!" });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Ep_Login_WithEmptyCredentials_ShouldNotReturnOk()
        {
            var req = BuildRequest(Method.Post, EndPointsSettings.ApiEndPoint.AuthLogin, withCorrelationHeaders: true);
            req.AddJsonBody(new LoginRequest { Username = string.Empty, Password = string.Empty });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK));
        }
    }
}
