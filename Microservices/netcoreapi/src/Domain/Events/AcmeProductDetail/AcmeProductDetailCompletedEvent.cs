using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class AcmeProductDetailCompletedEvent : DomainEvent
    {
        public AcmeProductDetailCompletedEvent(AcmeProduct AcmeProductDetailProduct)
        {
            AcmeProductDetailDetails = AcmeProductDetailProduct;
        }
        public AcmeProduct AcmeProductDetailDetails { get; }
    }
}
