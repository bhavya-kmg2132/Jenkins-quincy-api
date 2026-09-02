using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.SystemManager.EndPoints
{
    using static Testing;

    /// <summary>
    /// Integration tests for SystemManagerController
    ///   GET api/v1/SystemManager/GetAppInformationLogInJsonFormat
    ///   GET api/v1/SystemManager/GetRequestAndQueryNameList
    ///   GET api/v1/SystemManager/GetUnlistedRequestAndQueryNameList
    ///   GET api/v1/SystemManager/GetAppInformationLogFile  (file download — status only)
    ///   GET api/v1/SystemManager/GetAppErrorLogFile        (file download — status only)
    ///   GET api/v1/SystemManager/GetLogFile               (file download — status only)
    /// </summary>
    [TestFixture]
    public class EndpointSystemManagerTests : EndpointTestBase
    {
        private const string Base = "api/v1/SystemManager";

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
        public async Task Ep_SystemManager_GetAppLogInJsonFormat_ShouldReturnOk()
        {
            var req = Get($"{Base}/GetAppInformationLogInJsonFormat");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
        }

        [Test, Order(2)]
        public async Task Ep_SystemManager_GetRequestAndQueryNameList_ShouldReturnOk()
        {
            var req = Get($"{Base}/GetRequestAndQueryNameList");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
        }

        //[Test]
        //public async Task Ep_SystemManager_GetUnlistedRequestAndQueryNameList_ShouldReturnOk()
        //{
        //    var req = Get($"{Base}/GetUnlistedRequestAndQueryNameList");

        //    var response = await Client.ExecuteAsync(req);

        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        //}

        [Test, Order(3)]
        public async Task Ep_SystemManager_GetAppInformationLogFile_ShouldReturnOkOrNoContent()
        {
            var req = Get($"{Base}/GetAppInformationLogFile");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NoContent));
        }

        [Test, Order(4)]
        public async Task Ep_SystemManager_GetAppErrorLogFile_ShouldReturnOkOrNoContent()
        {
            var req = Get($"{Base}/GetAppErrorLogFile");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NoContent));
        }

        [Test, Order(5)]
        public async Task Ep_SystemManager_GetLogFile_ShouldReturnOkOrNoContent()
        {
            var req = Get($"{Base}/GetLogFile");

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NoContent));
        }
    }
}
