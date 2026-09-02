using System;
using System.Net;
using System.Threading.Tasks;
using Domain.Entities;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.Auth.EndPoints
{
    /// <summary>
    /// Integration tests for POST api/v1/Auth/register
    ///
    /// No shared user.  Every test that successfully creates a user stores the
    /// username in _perTestUsername; [TearDown] deactivates it so the DB stays
    /// clean between runs.
    /// </summary>
    [TestFixture]
    public class EndpointAuthRegisterTests : AuthTestBase
    {
        private string _perTestUsername;

        [TearDown]
        public async Task RegisterTearDown()
        {
            if (!string.IsNullOrEmpty(_perTestUsername))
            {
                await DeactivateUserByUsernameAsync(_perTestUsername);
                _perTestUsername = null;
            }
        }

        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public async Task Ep_Register_WithValidRequest_ShouldReturnOk()
        {
            _perTestUsername = $"reg.{Guid.NewGuid():N}@testdomain.com";

            var req = BuildRequest(Method.Post, EndPointsSettings.ApiEndPoint.AuthRegister, withCorrelationHeaders: true);
            req.AddJsonBody(new LoginRequest
            {
                Username = _perTestUsername,
                Password = "Test@Password123!",
                FirstName = "Integration",
                LastName = "Test",
                Mobile = "0000000000",
                auth_type = "db"
            });

            var response = await Client.ExecuteAsync(req);

            // [TearDown] deactivates _perTestUsername regardless of this assertion.
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_Register_WithMissingUsername_ShouldReturnBadRequest()
        {
            // No user is created on the server — no _perTestUsername to clean up.
            var req = BuildRequest(Method.Post, EndPointsSettings.ApiEndPoint.AuthRegister, withCorrelationHeaders: true);
            req.AddJsonBody(new LoginRequest
            {
                Username = null,
                Password = "Test@Password123!",
                FirstName = "Integration",
                LastName = "Test"
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
