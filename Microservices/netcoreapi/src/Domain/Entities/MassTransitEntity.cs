using System;

namespace Domain.Entities
{
    public class MassTransitEntity
    {
        public Guid OrderId { get; }

        public MassTransitEntity(Guid orderId)
        {
            OrderId = orderId;
        }

    }
}
