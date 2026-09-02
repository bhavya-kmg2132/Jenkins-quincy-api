using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.WeatherForecasts.Queries.GetWeatherForecasts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("[controller]")]
    public class WeatherForecastController : ApiControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherDataPublisher _weatherDataPublisher;
        public WeatherForecastController(IWebHostEnvironment env, ILogger<WeatherForecastController> logger, IWeatherDataPublisher weatherDataPublisher)
        {
            this._logger = logger;
            this._weatherDataPublisher = weatherDataPublisher;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                                            [FromHeader(Name = "X-Request-Id")] string requestId,
                                                            [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                                            [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                                            [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            _logger.LogInformation("Weather Forecast called!");
            return await Mediator.Send(new GetWeatherForecastsQuery());
        }

        [AllowAnonymous]
        [HttpPost("PostToKafka")]
        public async Task PostToKafka([FromHeader(Name = "X-Correlation-Id")] string correlationId,
                                      [FromHeader(Name = "X-Request-Id")] string requestId,
                                      [FromHeader(Name = "X-Request-Oid")] string requestOid,
                                      [FromHeader(Name = "X-Request-Uid")] string requestUid,
                                      [FromHeader(Name = "X-Api-Key")] string apiKey,
                                      [FromBody] WeatherForecast weather)
        {
            await this._weatherDataPublisher.ProduceAsync(weather);
        }
    }
}
