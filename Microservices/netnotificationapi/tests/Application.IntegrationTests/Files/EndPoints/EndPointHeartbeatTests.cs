using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using RestSharp;
using System.Reflection;
using Application.IntegrationTests.Utils;

namespace Application.IntegrationTests.EndPoints
{
    using static Testing;
    public class EndPointHeartbeatTests : EndpointTestBase
    {


        [SetUp]
        public void DerivedSetUp()
        {
            
        }

        [TearDown]
        public void DerivedTearDown() { }

        [Test]
        public async Task Ep_HeartbeatTest()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatEndPoint;
            Request.Method = Method.GET;
            Request.Resource = ServerUrl + EndPoint;
            //Request.RequestFormat = DataFormat.Json;
            //Request.AddHeader("Authorization", Auth_Token);
            //Request.AddHeader("Content-Type", "application/json");
            //Request.AddJsonBody(ContactUpdate);
            var response = Client.Execute(Request);
            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //{
            //    throw new ApplicationException("Error:" + response.ErrorMessage);
            //}

            //var postJsonResult = ((RestResponseBase)response).Content;
            //var OutputModel = JsonConvert.DeserializeObject<int>(postJsonResult);

            //Assertion
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);
        }

        [Test]
        public async Task Ep_Heartbeat_NotFoundResult_Test()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatNotFoundResultEndPoint;
            Request.Method = Method.GET;
            Request.Resource = ServerUrl + EndPoint;
            //Request.RequestFormat = DataFormat.Json;
            //Request.AddHeader("Authorization", Auth_Token);
            //Request.AddHeader("Content-Type", "application/json");
            //Request.AddJsonBody(ContactUpdate);
            var response = await Client.ExecuteAsync(Request);
            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //{
            //    throw new ApplicationException("Error:" + response.ErrorMessage);
            //}

            //var postJsonResult = ((RestResponseBase)response).Content;
            //var OutputModel = JsonConvert.DeserializeObject<int>(postJsonResult);

            //Assertion
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Ep_Heartbeat_NoContentResult_Test()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatNoContentResultEndPoint;
            Request.Method = Method.GET;
            Request.Resource = ServerUrl + EndPoint;
            var response = await Client.ExecuteAsync(Request);

            //Assertion
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.NoContent);
        }

        [Test]
        public async Task Ep_Heartbeat_BadRequestResult_Test()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatBadRequestResultEndPoint;
            Request.Method = Method.GET;
            Request.Resource = ServerUrl + EndPoint;
            var response = await Client.ExecuteAsync(Request);

            //Assertion
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Ep_Heartbeat_InternalServerErrorResult_Test()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatInternalServerErrorResultEndPoint;
            Request.Method = Method.GET;
            Request.Resource = ServerUrl + EndPoint;
            var response = await Client.ExecuteAsync(Request);

            //Assertion
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.BadGateway);
        }

        [Test]
        public async Task Ep_Heartbeat_UnprocessableEntityResult_Test()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatUnprocessableEntityResultEndPoint;
            Request.Method = Method.GET;
            Request.Resource = ServerUrl + EndPoint;
            var response = await Client.ExecuteAsync(Request);

            //Assertion
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.UnprocessableEntity);
        }

        [Test]
        public async Task Ep_Heartbeat_UnauthorizedResult_Test()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatUnauthorizedResultEndPoint;
            Request.Method = Method.GET;
            Request.Resource = ServerUrl + EndPoint;
            var response = await Client.ExecuteAsync(Request);

            //Assertion
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Ep_Heartbeat_OkResult_Test()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatOkResultEndPoint;
            Request.Method = Method.GET;
            Request.Resource = ServerUrl + EndPoint;
            var response = Client.Execute(Request);

            //Assertion
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);
        }
    }
}
