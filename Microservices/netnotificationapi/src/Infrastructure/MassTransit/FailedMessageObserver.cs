using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Dapper;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

public class FailurMessageObserver : IConsumeObserver
{
    private readonly ILogger<FailurMessageObserver> _logger;
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly string _sqlFilePath;
    private readonly Dictionary<string, string> _sqlQueries;

    public FailurMessageObserver(IConfiguration config, ILogger<FailurMessageObserver> logger)
    {
        _logger = logger;
        _config = config;
        _connectionString = _config["ConnectionStrings:AzureEventDBPostgreSqlDBConnection"];
        _sqlFilePath = _config["SqlSeparationQueries:PostgreNotification"];
        _sqlQueries = LoadSqlQueries();
    }

    private Dictionary<string, string> LoadSqlQueries()
    {
        string absoluteSqlFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _sqlFilePath));
        var xml = XElement.Load(absoluteSqlFilePath);

        return xml.Elements("sql")
                  .ToDictionary(e => e.Attribute("name").Value, e => e.Value.Trim());
    }

    public Task PreConsume<T>(ConsumeContext<T> context)
        where T : class
    {
        _logger.LogInformation(
            "PRE CONSUME -> Consumer started processing MessageType: {MessageType}, Queue: {QueueName}",
            typeof(T).Name,
            context.ReceiveContext.InputAddress.ToString());

        return Task.CompletedTask;
    }

    public Task PostConsume<T>(ConsumeContext<T> context)
        where T : class
    {
        _logger.LogInformation(
            "POST CONSUME -> Consumer successfully processed MessageType: {MessageType}, Queue: {QueueName}",
            typeof(T).Name,
            context.ReceiveContext.InputAddress.ToString());

        return Task.CompletedTask;
    }

    public async Task ConsumeFault<T>(
        ConsumeContext<T> context,
        Exception exception)
        where T : class
    {
        _logger.LogError(
            exception,
            "CONSUME FAULT -> Consumer failed processing MessageType: {MessageType}, Queue: {QueueName}",
            typeof(T).Name,
            context.ReceiveContext.InputAddress.ToString());

        var failedMessage = new FailedMessage
        {
            Id = Guid.NewGuid(),
            QueueName = context.ReceiveContext
                .InputAddress
                .ToString(),

            MessageType = typeof(T).FullName,

            Payload = JsonSerializer.Serialize(
                context.Message),

            Exception = exception.ToString(),

            FailedDateTime = DateTime.UtcNow
        };

        await AddAsync(failedMessage);
    }

    public async Task AddAsync(FailedMessage message)
    {
        try
        {
            string sql = _sqlQueries["PostgreNotification.FailedMassTransitMessages"];
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(sql, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist fault record for MessageId:{Id}", message.Id);
        }
    }
}