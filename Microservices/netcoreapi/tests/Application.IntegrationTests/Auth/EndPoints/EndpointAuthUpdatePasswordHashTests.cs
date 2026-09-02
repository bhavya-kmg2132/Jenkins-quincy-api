using System;
using System.Net;
using System.Threading.Tasks;
using Application.Users.Commands.UpdateUserPasswordHash;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.Auth.EndPoints
{
    using static Testing;

    [TestFixture]
    public class EndpointAuthUpdatePasswordHashTests : AuthTestBase
    {
        private string _perTestUsername;

        [TearDown]
        public async Task UpdatePasswordHashTearDown()
        {
            if (!string.IsNullOrEmpty(_perTestUsername))
            {
                await DeactivateUserByUsernameAsync(_perTestUsername);
                _perTestUsername = null;
            }
        }

        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public async Task Ep_UpdatePasswordHash_SameAsOld_ShouldBeRejectedByDomainRule()
        {
            // Arrange — isolated user so no other test is affected by the failure path
            _perTestUsername = $"pwdtest.{Guid.NewGuid():N}@testdomain.com";
            const string pwd = "PwdTest@123!";

            var registered = await RegisterUserViaEndpointAsync(_perTestUsername, pwd);
            Assert.That(registered, Is.True, "Could not register isolated password-test user.");

            var tokenModel = await LoginAsync(_perTestUsername, pwd);
            Assert.That(tokenModel?.AccessToken, Is.Not.Null, "Login must succeed before password update test.");

            var user = await _userDataAccess.GetUserFromNetAuthLibAsync(_perTestUsername);

            // Act — new password same as old → domain rule (ApplicationException) must reject
            var req = BuildRequest(Method.Post, EndPointsSettings.ApiEndPoint.AuthUpdatePasswordHash, withCorrelationHeaders: true);
            req.AddHeader("Authorization", $"Bearer {tokenModel.AccessToken}");
            req.AddJsonBody(new UpdateUserPasswordHashRequest
            {
                UserId = user?.Id,
                OldPassword = pwd,
                NewPassword = pwd,
                UpdateReason = "Integration test - same-password rejection"
            });

            var response = await Client.ExecuteAsync(req);

            // Domain rule raises ApplicationException → 4xx or 5xx, never 200
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Ep_UpdatePasswordHash_WithValidNewPassword_ShouldReturnOk()
        {
            // Arrange — isolated user
            _perTestUsername = $"pwdtest.{Guid.NewGuid():N}@testdomain.com";
            const string oldPwd = "OldPwd@123!";
            const string newPwd = "NewPwd@456!";

            var registered = await RegisterUserViaEndpointAsync(_perTestUsername, oldPwd);
            Assert.That(registered, Is.True, "Could not register isolated password-test user.");

            var tokenModel = await LoginAsync(_perTestUsername, oldPwd);
            Assert.That(tokenModel?.AccessToken, Is.Not.Null, "Login must succeed before password update test.");

            var user = await _userDataAccess.GetUserFromNetAuthLibAsync(_perTestUsername);

            // Act
            var req = BuildRequest(Method.Post, EndPointsSettings.ApiEndPoint.AuthUpdatePasswordHash, withCorrelationHeaders: true);
            req.AddHeader("Authorization", $"Bearer {tokenModel.AccessToken}");
            req.AddJsonBody(new UpdateUserPasswordHashRequest
            {
                UserId = user?.Id,
                OldPassword = oldPwd,
                NewPassword = newPwd,
                UpdateReason = "Integration test - valid password change"
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), $"Response body: {response.Content}");
        }
    }
}
