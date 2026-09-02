//using Application.Common.Models;
//using Confluent.Kafka;
//using Domain.Common;
//using Domain.Events;
//using MediatR;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Text.Json;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Application.AcmeProduct.EventHandlers
//{
//    /// <summary>
//    /// For Creating handler for the domain event notification,created AcmeProductCreatedEventHandler 
//    /// class that implements the INotificationHandler interface as shown below.
//    /// </summary>
//    public class AcmeProductCreatedEventHandler : INotificationHandler<DomainEventNotification<AcmeProductCreatedEvent>>
//    {
//        private readonly ILogger<AcmeProductCreatedEventHandler> _logger;
//        private IConfiguration _configuration;

//        /// <summary>
//        /// Instantiation of AcmeProductCreatedEventHandler class
//        /// </summary>
//        /// <param name="logger"></param>
//        /// <param name="configuration"></param>
//        public AcmeProductCreatedEventHandler(IConfiguration configuration, ILogger<AcmeProductCreatedEventHandler> logger)
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
//        public Task Handle(DomainEventNotification<AcmeProductCreatedEvent> notification, CancellationToken cancellationToken)
//        {
//            var domainEvent = notification.DomainEvent;
//            _logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);

//            if (Convert.ToBoolean(this._configuration["Kafka:UseKafka"]))
//            {
//                ProduceKafkaEvent(domainEvent);
//                ProduceKafkaEvent_topic_elastic_search(domainEvent);
//            }

//            return Task.CompletedTask;
//        }

//        private async void ProduceKafkaEvent(AcmeProductCreatedEvent domainEvent)
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

//            //Serialize object to send to KAFKA TOPIC
//            domainEvent.AcmeCreatedObject.AuditableSourceEventName = nameof(AcmeProductCreatedEvent);
//            string serializedObjectForTopic = JsonSerializer.Serialize(domainEvent.AcmeCreatedObject,
//                  new JsonSerializerOptions
//                  {
//                      WriteIndented = true,
//                      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//                      ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
//                  });

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

//        //publish event sent to topic_elastic_search  
//        private async void ProduceKafkaEvent_topic_elastic_search(AcmeProductCreatedEvent domainEvent)
//        {
//            var PublishEventData = PrepareEventDataFor_topic_elastic_search(domainEvent);
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

//            //Serialize object to send to KAFKA TOPIC
//            domainEvent.AcmeCreatedObject.AuditableSourceEventName = nameof(AcmeProductCreatedEvent);
//            string serializedObjectForTopic = JsonSerializer.Serialize(PublishEventData,
//            new JsonSerializerOptions
//            {
//                WriteIndented = true,
//                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
//            });

//            // If serializers are not specified, default serializers from
//            // `Confluent.Kafka.Serializers` will be automatically used where
//            // available. Note: by default strings are encoded as UTF8.
//            using (var producer = new ProducerBuilder<Null, string>(kafkaProducerConfig).Build())
//            {
//                try
//                {
//                    //Choose a partition to ensure message execution sequence in KAFKA
//                    var topicPartition = new TopicPartition(this._configuration["Kafka:topic_elastic_search"], new Partition(0));
//                    DeliveryResult<Null, string> deliveryResult = await producer.ProduceAsync(topicPartition, new Message<Null, string> { Value = serializedObjectForTopic });
//                }
//                catch (ProduceException<Null, string> exc)
//                {
//                    Console.WriteLine($"Kafka Delivery failed: {exc.Error.Reason}");
//                    _logger.LogError("Error Domain Event: {DomainEvent}, Error Message: {}", domainEvent.GetType().Name, exc.Message);
//                }
//            }
//        }

//        private PublishEventData PrepareEventDataFor_topic_elastic_search(AcmeProductCreatedEvent domainEvent)
//        {
//            //publish a event in "topic_elastic_search"
//            var acme = domainEvent.AcmeCreatedObject;
//            var eventData = new PublishEventData { Data = new List<Domain.Common.Property>() };
//            eventData.EventName = acme.AuditableSourceEventName;
//            eventData.OperationType = OperationType.INSERT;
//            eventData.OperationDateTimeUtc = DateTime.UtcNow;
//            eventData.OperationSource = OperationSource.WEBPAGE;
//            eventData.ApiName = this._configuration["Api:internal_name"];
//            eventData.CollectionName = "AcmeProduct";

//            //Get all the public and private members of the class/object
//            var objectProperties = Helper.RetrievePropertiesWithFilter(acme,
//                         BindingFlags.Instance |
//                         BindingFlags.Public |
//                         BindingFlags.NonPublic);

//            //Loop through all the members of the object and add them to eventData.
//            foreach (var item in objectProperties)
//            {
//                eventData.Data.Add(new Domain.Common.Property
//                {
//                    PropertyName = item.Name,
//                    NewValue = item.GetValue(acme, null),
//                    OldValue = null
//                });
//            }
//            //returns eventData
//            return eventData;
//        }
//    }
//}
