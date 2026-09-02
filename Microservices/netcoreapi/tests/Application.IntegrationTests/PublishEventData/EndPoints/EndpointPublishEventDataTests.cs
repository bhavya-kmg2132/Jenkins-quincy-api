using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.PublishEvent.Queries;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.PublishEventData.EndPoints
{
    using static Testing;

    /// <summary>
    /// Integration tests for PublishEventDataController
    ///   POST api/v1/PublishEventData/Getlist
    /// </summary>
    [TestFixture]
    public class EndpointPublishEventDataTests : EndpointTestBase
    {
        private static readonly JsonSerializerOptions JsonOpts =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private const string Base = "api/v1/PublishEventData";

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

        [Test, Order(1)]
        public async Task Ep_PublishEventData_GetList_WithValidFilter_ShouldReturnOk()
        {
            var req = Post($"{Base}/Getlist");
            req.AddJsonBody(new
            {
                PageNumber = 1,
                PageSize = 10,
                ColumnName = "EventName",
                OrderType = "asc",
                SearchText = "",
                FilterJson = ""
            });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

            var vm = JsonSerializer.Deserialize<PublishEventDataListVm>(response.Content, JsonOpts);
            Assert.That(vm, Is.Not.Null);
            Assert.That(vm.TotalCount, Is.GreaterThanOrEqualTo(0));
            Assert.That(vm.PublishEventData, Is.Not.Null);
        }

        [Test, Order(2)]
        public async Task Ep_PublishEventData_GetList_WithEmptyBody_ShouldNotCrash()
        {
            var req = Post($"{Base}/Getlist");
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }
    }
}
