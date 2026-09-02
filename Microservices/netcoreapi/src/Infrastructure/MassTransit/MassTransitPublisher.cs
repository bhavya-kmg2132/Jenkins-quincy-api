using System;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using MassTransit;
using Messaging.Contract;
using Messaging.Contract.Notification;
using Newtonsoft.Json;

public class MassTransitPublisher : IMassTransitPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly ICurrentUserService _currentUserService;

    public MassTransitPublisher(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider, ICurrentUserService currentUserService)
    {
        _publishEndpoint = publishEndpoint;
        _sendEndpointProvider = sendEndpointProvider;
        _currentUserService = currentUserService;
    }

    public async Task PublishEventAsync(object data, string type)
    {
        //For Zepto Notification
        await _publishEndpoint.Publish(new CreateNotificationMessage
        {
            NotificationId = Guid.NewGuid(),
            Type = type,
            PayloadJson = JsonConvert.SerializeObject(data),
            CreatedAtUtc = DateTime.UtcNow
        });

        // For Message Envelope
        await _publishEndpoint.Publish(new MessageEnvelope<string>("Test payload for Message Envelop Consumer Data Contract", _currentUserService.CorrelationId)
        {
            Payload = JsonConvert.SerializeObject(data)
        });

        //var endpoint = await _sendEndpointProvider.GetSendEndpoint(
        //new Uri("queue:create-notification-queue-local"));

        //await endpoint.Send(new CreateNotificationMessage
        //{
        //    NotificationId = Guid.NewGuid(),
        //    Type = type,
        //    PayloadJson = JsonConvert.SerializeObject(data),
        //    CreatedAtUtc = DateTime.UtcNow
        //});
    }
}
