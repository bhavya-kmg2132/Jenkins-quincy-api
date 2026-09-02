using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Api.Entities;
using Api.Utilities;
using Confluent.Kafka;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Api.Services
{
    public class ApiWorkerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ApiWorkerService> _logger;
        private readonly IConfiguration _config;

        // 300000 ms = 300 s = 5 min
        // 120000 ms = 120 s = 2 min
        // 60000 ms = 60 s = 1 min
        // 30000 ms = 30 s
        // 20000 ms = 20 s
        private readonly System.Int32 BackgroungJobExecutionDelayInMilliSeconds = 30000;
        private readonly System.Int32 KafkaConsumerTimeoutPeriodInMilliSeconds = 20000;
        public ApiWorkerService(IServiceProvider serviceProvider, ILogger<ApiWorkerService> logger, IConfiguration config)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (KafkaConsumerTimeoutPeriodInMilliSeconds > BackgroungJobExecutionDelayInMilliSeconds)
            {
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (IServiceScope scope = _serviceProvider.CreateScope())
                    {
                        //Task 1
                        await InsertApiWorkerServiceLog();
                    }


                    //Task 2
                    if (Convert.ToBoolean(this._config["Kafka:UseKafka"]))
                    {
                        await KafkaConsumerPoll(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical("ApiWorkerService Exception at: {time}: {exception}", DateTime.UtcNow.ToString(), ex.Message);
                }

                await Task.Delay(BackgroungJobExecutionDelayInMilliSeconds, stoppingToken);
            }
        }

        public async Task<int> InsertApiWorkerServiceLog()
        {
            try
            {
                if (!TableExists("ApiWorkerServiceLog"))
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(this._config["ConnectionStrings:SqlDBConnection"]);
                    string database = builder.InitialCatalog;
                    SqlConnection conn = new SqlConnection(this._config["ConnectionStrings:SqlDBConnection"]);
                    string strCmd = "CREATE TABLE ApiWorkerServiceLog (Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, JobKey varchar(200), JobName varchar(200), CreatedDateTime DateTime Default GETUTCDATE(), ModifiedDateTime DateTime, JobSource varchar(200), JobStatus varchar(200), Attempts int DEFAULT 0, JsonText varchar(max))";
                    SqlCommand sqlCmd = new SqlCommand(strCmd, conn);
                    SqlHelper.ExecuteScalar(conn, CommandType.Text, strCmd);
                }

                string query = "INSERT INTO ApiWorkerServiceLog (JobKey, JobName, JobSource, JobStatus, Attempts, ModifiedDateTime, JsonText ) VALUES (@JobKey, @JobName, @JobSource, @JobStatus, @Attempts, @ModifiedDateTime, @JsonText); SELECT SCOPE_IDENTITY()";
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@JobKey", "key-" + this._config["Api:internal_name"]));
                parameters.Add(new SqlParameter("@JobName", "name-" + this._config["Api:internal_name"]));
                parameters.Add(new SqlParameter("@JobSource", this._config["Api:internal_name"]));
                parameters.Add(new SqlParameter("@JobStatus", "Completed"));
                parameters.Add(new SqlParameter("@Attempts", Convert.ToInt16(1)));
                parameters.Add(new SqlParameter("@ModifiedDateTime", DateTime.UtcNow));
                parameters.Add(new SqlParameter("@JsonText", @"{'Backgroung Job Execution Delay In Milli Seconds': " + BackgroungJobExecutionDelayInMilliSeconds + ", 'Source': '" + this._config["Api:internal_name"] + "'}"));
                //int rowsAffected = await SqlHelper.ExecuteNonQueryAsync(constr, CommandType.Text, query, parameters.ToArray());
                int insertedID = Convert.ToInt32(await SqlHelper.ExecuteScalarAsync(this._config["ConnectionStrings:SqlDBConnection"], CommandType.Text, query, parameters.ToArray()));

                return insertedID;
            }
            catch (Exception ex)
            {
                _logger.LogError("AddBackgroundJobLog (InsertApiWorkerServiceLog) - " + ex.Message);
                return 0;
            }
        }

        public async Task KafkaConsumerPoll(CancellationToken stoppingToken)
        {
            var kafkaConsumerConfig = new ConsumerConfig
            {

                GroupId = this._config["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BootstrapServers = this._config["Kafka:BootstrapServers"],
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = this._config["Kafka:SaslUsername"],
                SaslPassword = this._config["Kafka:SaslPassword"],
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<Null, string>(kafkaConsumerConfig).Build();
            consumer.Subscribe(this._config["Kafka:Topic"]);
            string consumeResultString = string.Empty;
            try
            {
                //Poll for consumer message from Kafka
                var response = consumer.Consume(stoppingToken); //KafkaConsumerTimeoutPeriodInMilliSeconds
                if (response != null)
                {
                    consumeResultString = response.Message.Value;
                    // consumer.Commit(response);

                    // Convert message in JsonObject
                    JObject jsonObject = JObject.Parse(consumeResultString);

                    // Extract the value of the "notification_type" property
                    if (jsonObject["notification_type"].Value<string>() != null)
                    {
                        string notification_type = jsonObject["notification_type"].Value<string>();

                        if (notification_type == "email")
                        {
                            var notification = JsonSerializer.Deserialize<Notification>(consumeResultString);
                            Endpoint.HttpPostRequest(this._config["ServerURL:Notification"], this._config["EndPoint:SendEmailNotification"], notification);
                        }

                        /*   else if (notification_type == "sms")
                          {
                              var notification = JsonSerializer.Deserialize<Notification>(consumeResultString);
                              Endpoint.HttpPostRequest(this._config["ServerURL:local"], this._config["EndPoint:SendSMSNotification"], notification);
                          } */

                        Console.WriteLine($"Consumed message '{response.Message}' at: '{response.TopicPartitionOffset}'.");
                        //_logger.LogInformation($"Consumed message '{response.Message}' at: '{response.TopicPartitionOffset}'.");
                        consumer.Commit(response);
                    }
                    consumer.Commit(response);

                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError("AddBackgroundJobLog(KafkaConsumerPoll) - " + ex.Message);
            }

            // DO action based on Kafka message
            await ConsumerAction(consumeResultString);
        }

        private async Task ConsumerAction(string consumeResultString)
        {
            //If consumer is able to get data the do next actions
            if (!string.IsNullOrEmpty(consumeResultString))
            {
                try
                {
                    string query = "INSERT INTO ApiWorkerServiceLog (JobKey, JobName, JobSource, JobStatus, Attempts, ModifiedDateTime, JsonText ) VALUES (@JobKey, @JobName, @JobSource, @JobStatus, @Attempts, @ModifiedDateTime, @JsonText); SELECT SCOPE_IDENTITY()";
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("@JobKey", @"KAFKA_" + DateTime.UtcNow.ToString("yyyyMMdd") + "_" + Guid.NewGuid()));
                    parameters.Add(new SqlParameter("@JobName", "KAFKA-CONSUMER-" + this._config["Api:internal_name"]));
                    parameters.Add(new SqlParameter("@JobSource", "KAFKA-" + this._config["Api:internal_name"]));
                    parameters.Add(new SqlParameter("@JobStatus", "Completed"));
                    parameters.Add(new SqlParameter("@Attempts", Convert.ToInt16(1)));
                    parameters.Add(new SqlParameter("@ModifiedDateTime", DateTime.UtcNow));
                    parameters.Add(new SqlParameter("@JsonText", @"{'DataFound': " + DateTime.UtcNow.ToString() + ", 'Source': 'ExecuteKafkaConsumer'" + ", 'Data':" + " '" + "consumer result = " + consumeResultString + "'" + "}"));
                    //int rowsAffected = await SqlHelper.ExecuteNonQueryAsync(constr, CommandType.Text, query, parameters.ToArray());
                    int insertedID = Convert.ToInt32(await SqlHelper.ExecuteScalarAsync(this._config["ConnectionStrings:SqlDBConnection"], CommandType.Text, query, parameters.ToArray()));
                }
                catch (Exception ex)
                {
                    _logger.LogError("AddBackgroundJobLog(ConsumerAction) - " + ex.Message);
                }
            }
        }

        private bool TableExists(string tableName)
        {
            bool isExists = false;
            string schemaName = "dbo";
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(this._config["ConnectionStrings:SqlDBConnection"]);
            string database = builder.InitialCatalog;
            SqlConnection conn = new SqlConnection(this._config["ConnectionStrings:SqlDBConnection"]);
            string strCmd = null;
            SqlCommand sqlCmd = null;

            try
            {
                strCmd = "select case when exists((select '['+SCHEMA_NAME(schema_id)+'].[" + schemaName + "]' As name FROM [" + database + "].sys.tables WHERE name = '" + tableName + "')) then 1 else 0 end";
                using (conn)
                {
                    conn.Open();
                    sqlCmd = new SqlCommand(strCmd, conn);
                    isExists = (int)sqlCmd.ExecuteScalar() == 1;
                    conn.Close();
                }

                return isExists;
            }
            catch
            {
                return false;
            }
        }
    }
}
