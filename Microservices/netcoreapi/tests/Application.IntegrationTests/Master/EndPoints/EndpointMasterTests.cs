using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.Master.EndPoints
{
    using static Testing;

    /// <summary>
    /// Integration tests for MasterController
    ///   POST api/v1/Master/GetFilteredGenericMasterTable
    /// </summary>
    [TestFixture]
    public class EndpointMasterTests : EndpointTestBase
    {
        private const string Base = "api/v1/Master";

        private RestRequest Post(string path)
        {
            var req = new RestRequest
            {
                Method = Method.Post,
                Resource = ServerUrl + path,
                RequestFormat = DataFormat.Json
            };
            req.AddHeader("X-Correlation-Id", Guid.NewGuid().ToString());
            req.AddHeader("X-Request-Id", Guid.NewGuid().ToString());
            req.AddHeader("X-Request-Uid", RequestUid);
            req.AddHeader("X-Api-Key", ApiKey);
            return req;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GetFilteredGenericMasterTable
        // ─────────────────────────────────────────────────────────────────────

        [Test, Order(1)]
        public async Task Ep_Master_GetFilteredGenericMasterTable_WithValidTable_ShouldReturnOk()
        {
            var req = Post($"{Base}/GetFilteredGenericMasterTable");
            req.AddJsonBody(new
            {
                TableName = "AcmeProduct",
                PageSize = 10,
                PageNumber = 1
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NotFound));
        }

        [Test, Order(2)]
        public async Task Ep_Master_GetFilteredGenericMasterTable_WithEmptyTableName_ShouldNotCrash()
        {
            var req = Post($"{Base}/GetFilteredGenericMasterTable");
            req.AddJsonBody(new
            {
                TableName = string.Empty,
                PageSize = 10,
                PageNumber = 1
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }

        [Test, Order(3)]
        public async Task Ep_Master_GetFilteredGenericMasterTable_WithEmptyBody_ShouldNotCrash()
        {
            var req = Post($"{Base}/GetFilteredGenericMasterTable");
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }
    }
}
