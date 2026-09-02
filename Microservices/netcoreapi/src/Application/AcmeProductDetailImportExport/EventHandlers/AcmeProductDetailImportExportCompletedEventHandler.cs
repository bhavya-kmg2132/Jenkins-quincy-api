//using Application.Common.Models;
//using Confluent.Kafka;
//using Domain.Events;
//using MediatR;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Application.AcmeProduct.EventHandlers
//{
//    /// <summary>
//    /// For Creating handler for the domain event notification,created AcmeProductCompletedEventHandler 
//    /// class that implements the INotificationHandler interface as shown below.
//    /// </summary>
//    public class AcmeProductCompletedEventHandler : INotificationHandler<DomainEventNotification<AcmeProductCompletedEvent>>
//    {
//        private readonly ILogger<AcmeProductCompletedEventHandler> _logger;
//        private IConfiguration _configuration;

//        /// <summary>
//        /// Instantiation of AcmeProductCompletedEventHandler class
//        /// </summary>
//        /// <param name="logger"></param>
//        public AcmeProductCompletedEventHandler(IConfiguration configuration, ILogger<AcmeProductCompletedEventHandler> logger)
//        {
//            this._configuration = configuration;
//            this._logger = logger;
//        }

//        /// <summary>
//        /// Handler will recieve notification ,process it and will return the response. 
//        /// </summary>
//        /// <param name="notification"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns>Task.CompletedTask</returns>
//        public Task Handle(DomainEventNotification<AcmeProductCompletedEvent> notification, CancellationToken cancellationToken)
//        {
//            var domainEvent = notification.DomainEvent;

//            _logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);

//            if (Convert.ToBoolean(this._configuration["Kafka:UseKafka"]))
//            {
//                ProduceKafkaEvent(domainEvent);
//            }

//            return Task.CompletedTask;
//        }

//        private async void ProduceKafkaEvent(AcmeProductCompletedEvent domainEvent)
//        {
//            //KAFKA
//            var kafkaProducerConfig = new ProducerConfig
//            {
//                //BootstrapServers = "localhost:9092"\
//                BootstrapServers = this._configuration["Kafka:BootstrapServers"],
//                SecurityProtocol = SecurityProtocol.SaslSsl,
//                SaslMechanism = SaslMechanism.Plain,
//                SaslUsername = this._configuration["Kafka:SaslUsername"],
//                SaslPassword = this._configuration["Kafka:SaslPassword"]
//            };

//string serializedObjectForTopic = JsonSerializer.Serialize(domainEvent.AcmeCreatedObject,
//      new JsonSerializerOptions
//      {
//          WriteIndented = true,
//          PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//          ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
//      });

//            // If serializers are not specified, default serializers from
//            // `Confluent.Kafka.Serializers` will be automatically used where
//            // available. Note: by default strings are encoded as UTF8.
//            using (var producer = new ProducerBuilder<Null, string>(kafkaProducerConfig).Build())
//            {
//                try
//                {
//                    //Choose a partition to ensure message execution sequence in KAFKA
//                    var topicPartition = new TopicPartition(this._configuration["Kafka:Topic"], new Partition(0));
//                    DeliveryResult<Null, string> deliveryResult = await producer.ProduceAsync(topicPartition, new Message<Null, string> { Value = serializedObjectForTopic });
//                }
//                catch (ProduceException<Null, string> exc)
//                {
//                    Console.WriteLine($"Kafka Delivery failed: {exc.Error.Reason}");
//                    _logger.LogError("Error Domain Event: {DomainEvent}, Error Message: {}", domainEvent.GetType().Name, exc.Message);
//                }
//            }
//        }
//    }
//}
