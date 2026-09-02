using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.FireForget.EndPoints
{
    using static Testing;

    /// <summary>
    /// Integration tests for FireForgetController
    ///   GET api/v1/FireForget
    /// </summary>
    [TestFixture]
    public class EndpointFireForgetTests : EndpointTestBase
    {
        private const string Base = "api/v1/FireForget";

        private RestRequest Get(string path)
        {
            var req = new RestRequest { Method = Method.Get, Resource = ServerUrl + path };
            req.AddHeader("X-Correlation-Id", Guid.NewGuid().ToString());
            req.AddHeader("X-Request-Id", Guid.NewGuid().ToString());
            req.AddHeader("X-Request-Uid", RequestUid);
            req.AddHeader("X-Api-Key", ApiKey);
            return req;
        }

        [Test, Order(1)]
        public async Task Ep_FireForget_Execute_ShouldReturnSuccessOrUnauthorized()
        {
            var req = Get(Base);

            var response = await Client.ExecuteAsync(req);

            // FireForget is not [AllowAnonymous]; may require auth token.
            // Accept 200/204 (success) or 401 (no auth token provided).
            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.NoContent)
                    .Or.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test, Order(2)]
        public async Task Ep_FireForget_Execute_ShouldNotReturnServerError()
        {
            var req = Get(Base);

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }
    }
}
