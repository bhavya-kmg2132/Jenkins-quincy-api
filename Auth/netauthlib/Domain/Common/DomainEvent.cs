namespace NetAuth.Domain.Common
{
    /// <summary>
    /// Template for entity Domain Event
    /// </summary>
    internal interface IHasDomainEvent
    {
        public List<DomainEvent> DomainEvents { get; set; }
    }

    internal abstract class DomainEvent
    {
        protected DomainEvent()
        {
            DateOccurred = DateTimeOffset.UtcNow;
        }
        public bool IsPublished { get; set; }
        public DateTimeOffset DateOccurred { get; protected set; } = DateTime.UtcNow;
    }
}
