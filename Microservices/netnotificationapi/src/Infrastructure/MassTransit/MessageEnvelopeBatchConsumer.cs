using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Application.Common.Interfaces;
using Dapper;
using Dapper.Extensions;
using MassTransit;
using Messaging.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

public class MessageEnvelopeBatchConsumer<T> : IConsumer<Batch<MessageEnvelope<T>>>
{
    private readonly ILogger<MessageEnvelopeBatchConsumer<T>> _logger;
    private readonly IConfiguration _config;
    private readonly IEndpoint _endpoint;

    private readonly IDapper _dapper;
    private readonly string _sqlFilePath;
    private readonly Dictionary<string, string> _sqlQueries;
    private IDbConnection _dapperDbConnection { get; set; }
    private readonly IEmailNotificationService _emailNotificationService;
    public MessageEnvelopeBatchConsumer(ILogger<MessageEnvelopeBatchConsumer<T>> logger, IConfiguration config, IEndpoint endpoint, IDapper dapper, IEmailNotificationService emailNotificationService)
    {
        _logger = logger;
        _config = config;
        _endpoint = endpoint;
        _sqlFilePath = _config["SqlSeparationQueries:PostgreNotification"];
        _sqlQueries = LoadSqlQueries();
        _dapperDbConnection = new NpgsqlConnection(_config["ConnectionStrings:AzureEventDBPostgreSqlDBConnection"]);
        this._dapper = dapper;
        _emailNotificationService = emailNotificationService;
    }

    private Dictionary<string, string> LoadSqlQueries()
    {
        string absoluteSqlFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _sqlFilePath));
        var xml = XElement.Load(absoluteSqlFilePath);

        return xml.Elements("sql")
                  .ToDictionary(e => e.Attribute("name").Value, e => e.Value.Trim());
    }
    public async Task Consume(ConsumeContext<Batch<MessageEnvelope<T>>> context)
    {
        //await Task.Delay(10000);

        for (int i = 0; i < context.Message.Length; i++)
        {
            ConsumeContext<MessageEnvelope<T>> item = context.Message[i];

            try
            {
                await MassTransitServiceLogs(item);
            }

            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Error occured in Mass Transit Service for MessageEnvelope: {ex.Message}");
            }
        }
    }

    public async Task MassTransitServiceLogs(ConsumeContext<MessageEnvelope<T>> context)
    {
        var envelope = context.Message;

        var log = new
        {
            Id = context.MessageId?.ToString(),
            CorrelationId = envelope.CorrelationId,
            ConversationId = context.ConversationId.ToString(),
            MessageType = typeof(T).Name,
            SourceAddress = context.SourceAddress?.ToString(),
            DestinationAddress = context.DestinationAddress?.ToString(),
            Payload = envelope.Payload
        };

        using var connection = new NpgsqlConnection(_config["ConnectionStrings:AzureEventDBPostgreSqlDBConnection"]);

        await connection.ExecuteScalarAsync<string>(
            _sqlQueries["PostgreNotification.MassTransitServiceLogs"], log);

        _logger.LogInformation("PublishEventDataAccess.Add - Completed");
    }
}

