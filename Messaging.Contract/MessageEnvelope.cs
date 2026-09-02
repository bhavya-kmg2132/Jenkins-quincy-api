namespace Messaging.Contract;

public record MessageEnvelope<T>
{
    public string? MessageId { get; init; }
    public string CorrelationId { get; init; }
    public string? CausationId { get; init; }

    public string MessageType { get; init; } = typeof(T).Name;
    public string? MessageName { get; init; }
    public string? Version { get; init; } = "1.0";

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public T Payload { get; init; }

    public Dictionary<string, string> Headers { get; init; } = new();

    public string? Source { get; init; }
    public string? Environment { get; init; }

    public int RetryCount { get; init; } = 0;

    public MessageEnvelope(T payload, string correlationId)
    {
        Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        CorrelationId = correlationId ?? Guid.NewGuid().ToString();
    }

    //public MessageEnvelope<T> WithHeader(string key, string value)
    //{
    //    var newHeaders = new Dictionary<string, string>(Headers)
    //    {
    //        [key] = value
    //    };

    //    return this with { Headers = newHeaders };
    //}

    //public MessageEnvelope<T> WithSource(string source)
    //    => this with { Source = source };

    //public MessageEnvelope<T> WithRetry(int retryCount)
    //    => this with { RetryCount = retryCount };
}