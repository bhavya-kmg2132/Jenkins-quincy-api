public class FailedMessage
{
    public Guid Id { get; set; }

    public string? QueueName { get; set; }

    public string? MessageType { get; set; }

    public string? Payload { get; set; }

    public string? Exception { get; set; }

    public DateTime FailedDateTime { get; set; }
}