using System;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.WeatherForecasts.Queries.GetWeatherForecasts;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Api.Services
{
    public class WeatherDataPublisher : IWeatherDataPublisher
    {
        private readonly IProducer<Null, string> producer;
        private readonly IConfiguration _configuration;
        public WeatherDataPublisher(IConfiguration configuration, IProducer<Null, string> producer)
        {
            this._configuration = configuration;
            this.producer = producer;
        }


        public async Task ProduceAsync(WeatherForecast weather)
        {
            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using (producer)
            {
                try
                {
                    weather.AuditableSourceEventName = "event_weather_created";
                    var dr = await producer.ProduceAsync(this._configuration["Kafka:Topic"], new Message<Null, string> { Value = JsonSerializer.Serialize(weather) });
                    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }

        /*
       Note that a server round-trip is slow (3ms at a minimum; actual latency depends on many factors). 
       In highly concurrent scenarios you will achieve high overall throughput out of the producer using 
       the above approach, but there will be a delay on each await call. 
       In stream processing applications, where you would like to process many messages in rapid succession, 
       you would typically use the Produce method instead
        */
        public void Produce(WeatherForecast weather)
        {
            var conf = new ProducerConfig { BootstrapServers = "localhost:9092" };

            Action<DeliveryReport<Null, string>> handler = r =>
                Console.WriteLine(!r.Error.IsError
                    ? $"Delivered message to {r.TopicPartitionOffset}"
                    : $"Delivery Error: {r.Error.Reason}");

            using (producer)
            {
                for (int i = 0; i < 100; ++i)
                {
                    producer.Produce(this._configuration["Kafka:Topic"], new Message<Null, string> { Value = i.ToString() }, handler);
                }

                // wait for up to 10 seconds for any inflight messages to be delivered.
                producer.Flush(System.TimeSpan.FromSeconds(10));
            }
        }
    }


}
