using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.EndPoints
{
    public class EndPointHeartbeatTests : EndpointTestBase
    {


        [SetUp]
        public void DerivedSetUp()
        {

        }

        [TearDown]
        public void DerivedTearDown() { }

        [Test, Order(1)]
        public async Task Ep_HeartbeatTest()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatEndPoint;
            Request.Method = Method.Get;
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
            // var OutputModel = JsonSerializer.Deserialize<int>(postJsonResult);

            //Assertion
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        }

        [Test, Order(2)]
        public async Task Ep_Heartbeat_NotFoundResult_Test()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatNotFoundResultEndPoint;
            Request.Method = Method.Get;
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
            //var OutputModel = JsonSerializer.Deserialize<int>(postJsonResult);

            //Assertion
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
        }

        [Test, Order(3)]
        public async Task Ep_Heartbeat_NoContentResult_Test()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatNoContentResultEndPoint;
            Request.Method = Method.Get;
            Request.Resource = ServerUrl + EndPoint;
            var response = await Client.ExecuteAsync(Request);

            //Assertion
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NoContent));
        }

        [Test, Order(4)]
        public async Task Ep_Heartbeat_BadRequestResult_Test()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatBadRequestResultEndPoint;
            Request.Method = Method.Get;
            Request.Resource = ServerUrl + EndPoint;
            var response = await Client.ExecuteAsync(Request);

            //Assertion
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
        }

        //[Test]
        //public async Task Ep_Heartbeat_InternalServerErrorResult_Test()
        //{
        //    EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatInternalServerErrorResultEndPoint;
        //    Request.Method = Method.Get;
        //    Request.Resource = ServerUrl + EndPoint;
        //    var response = await Client.ExecuteAsync(Request);

        //    //Assertion
        //    Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadGateway));
        //}

        [Test, Order(5)]
        public async Task Ep_Heartbeat_UnprocessableEntityResult_Test()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatUnprocessableEntityResultEndPoint;
            Request.Method = Method.Get;
            Request.Resource = ServerUrl + EndPoint;
            var response = await Client.ExecuteAsync(Request);

            //Assertion
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.UnprocessableEntity));
        }

        [Test, Order(6)]
        public async Task Ep_Heartbeat_UnauthorizedResult_Test()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatUnauthorizedResultEndPoint;
            Request.Method = Method.Get;
            Request.Resource = ServerUrl + EndPoint;
            var response = await Client.ExecuteAsync(Request);

            //Assertion
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Unauthorized));
        }

        [Test, Order(7)]
        public async Task Ep_Heartbeat_OkResult_Test()
        {
            EndPoint = EndPointsSettings.ApiEndPoint.GetHeartbeatOkResultEndPoint;
            Request.Method = Method.Get;
            Request.Resource = ServerUrl + EndPoint;
            var response = await Client.ExecuteAsync(Request);

            //Assertion
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        }
    }
}
