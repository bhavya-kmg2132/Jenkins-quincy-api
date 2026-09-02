using Domain.Common;
using Domain.Entities;

namespace Domain.Events
{
    public class AcmeProductDetailCreatedEvent : DomainEvent
    {
        public AcmeProductDetailCreatedEvent(AcmeProduct AcmeProductDetails)
        {
            AcmeProductDetail = AcmeProductDetails;
        }
        public AcmeProduct AcmeProductDetail { get; }
    }
}
