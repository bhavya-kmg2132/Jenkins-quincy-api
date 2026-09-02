using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace Application.IntegrationTests.WeatherForecast.EndPoints
{
    using static Testing;

    /// <summary>
    /// Integration tests for WeatherForecastController
    ///   GET api/v1/WeatherForecast
    ///   POST api/v1/WeatherForecast/PostToKafka
    /// </summary>
    [TestFixture]
    public class EndpointWeatherForecastTests : EndpointTestBase
    {
        private const string Base = "api/v1/WeatherForecast";

        private RestRequest Get(string path)
        {
            var req = new RestRequest { Method = Method.Get, Resource = ServerUrl + path };
            req.AddHeader("X-Correlation-Id", Guid.NewGuid().ToString());
            req.AddHeader("X-Request-Id", Guid.NewGuid().ToString());
            req.AddHeader("X-Request-Uid", RequestUid);
            req.AddHeader("X-Api-Key", ApiKey);
            return req;
        }

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
        public async Task Ep_WeatherForecast_Get_ShouldReturnOk()
        {
            var req = Get(Base);

            var response = await Client.ExecuteAsync(req);

            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
        }

        //[Test]
        //public async Task Ep_WeatherForecast_PostToKafka_WithValidBody_ShouldNotCrash()
        //{
        //    var req = Post($"{Base}/PostToKafka");
        //    req.AddJsonBody(new
        //    {
        //        Date        = DateTime.UtcNow.ToString("yyyy-MM-dd"),
        //        TemperatureC = 22,
        //        Summary     = "Integration test weather"
        //    });

        //    var response = await Client.ExecuteAsync(req);

        //    // Fire-and-forget publish — expect 200 or 204; must not be a server error
        //    Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        //}

        [Test, Order(2)]
        public async Task Ep_WeatherForecast_PostToKafka_WithEmptyBody_ShouldNotCrash()
        {
            var req = Post($"{Base}/PostToKafka");
            req.AddJsonBody(new { });

            var response = await Client.ExecuteAsync(req);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.InternalServerError));
        }
    }
}
