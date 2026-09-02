using System.Threading.Tasks;
using Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace Application.Common.Interfaces
{
    public interface IWeatherDataPublisher
    {
        public Task ProduceAsync(WeatherForecast weather);
        public void Produce(WeatherForecast weather);
    }
}
