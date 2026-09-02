using NetAuth.Domain.Common;

namespace NetAuth.Interfaces
{
    internal interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}
