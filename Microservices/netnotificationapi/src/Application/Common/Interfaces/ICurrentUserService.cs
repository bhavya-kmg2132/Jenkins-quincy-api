namespace Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        public string CorrelationId { get; }
        public string RequestId { get; }

    }
}
