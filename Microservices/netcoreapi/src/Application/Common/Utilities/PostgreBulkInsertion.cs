using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;

namespace Application.Common.Utilities
{
    public class PostgreBulkInsertion : IPostgreBulkInsertion
    {
        private IConfiguration _configuration;

        public PostgreBulkInsertion(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task BulkInsertNotificationsAsync(IEnumerable<PostgreNotification> notifications)
        {
            var connectionString = _configuration["ConnectionStrings:PostgreSqlDBConnection"];

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Begin bulk insert using PostgreSQL COPY binary format
            using var writer = connection.BeginBinaryImport(@"
        COPY ""NotificationRequest"" (
            ""Id"", ""ApiKey"", ""CreatedDateTime"", ""UpdatedDateTime"", ""EmailFrom"",
            ""EmailTo"", ""EmailCc"", ""EmailBcc"", ""EmailSubject"", ""EmailBody"",
            ""EmailAttachments"", ""NotificationErrorMessage"", ""NotificationDelivery"",
            ""ScheduledDateTime"", ""NotificationResponseDateTime"", ""EntityJson"",
            ""CreatedBy"", ""CreatedById"", ""UpdatedBy"", ""UpdatedById"", ""UpdateReason"",
            ""OwnerId"", ""IsActive"", ""IsDeleted"", ""IsApproved"", ""ApproverId"",
            ""ApprovedDateTime"", ""IsAuthorized"", ""AuthorizedById"", ""AuthorizedDateTime"",
            ""SysData"", ""TenantId"", ""AssociatedUserId"", ""SubTenantId"", ""CustomFields""
        )
        FROM STDIN (FORMAT BINARY)");

            foreach (var n in notifications)
            {
                await writer.StartRowAsync();

                writer.Write(n.Id ?? Guid.NewGuid().ToString(), NpgsqlDbType.Text);
                writer.Write(n.ApiKey, NpgsqlDbType.Text);
                writer.Write(DateTime.SpecifyKind(n.CreatedDateTime ?? DateTime.UtcNow, DateTimeKind.Unspecified), NpgsqlDbType.Timestamp);
                writer.Write(DateTime.SpecifyKind(n.UpdatedDateTime ?? DateTime.UtcNow, DateTimeKind.Unspecified), NpgsqlDbType.Timestamp);

                writer.Write(n.EmailFrom, NpgsqlDbType.Text);
                writer.Write(n.EmailTo, NpgsqlDbType.Text);
                writer.Write(n.EmailCc, NpgsqlDbType.Text);
                writer.Write(n.EmailBcc, NpgsqlDbType.Text);
                writer.Write(n.EmailSubject, NpgsqlDbType.Text);
                writer.Write(n.EmailBody, NpgsqlDbType.Text);

                writer.Write(JsonConvert.SerializeObject(n.EmailAttachments ?? new List<EmailAttachment>()), NpgsqlDbType.Jsonb);
                writer.Write(n.NotificationErrorMessage, NpgsqlDbType.Text);
                writer.Write(JsonConvert.SerializeObject(n.NotificationDelivery ?? new NotificationStatus()), NpgsqlDbType.Jsonb);

                if (n.ScheduledDateTime.HasValue)
                    writer.Write(DateTime.SpecifyKind(n.ScheduledDateTime.Value, DateTimeKind.Unspecified), NpgsqlDbType.Timestamp);
                else
                    writer.WriteNull();
                writer.Write(DateTime.SpecifyKind(n.NotificationResponseDateTime, DateTimeKind.Unspecified), NpgsqlDbType.Timestamp);

                writer.Write(JsonConvert.SerializeObject(n.EntityJson ?? new NotificationEntityJson()), NpgsqlDbType.Jsonb);

                // Audit Fields
                writer.Write(n.CreatedBy, NpgsqlDbType.Text);
                writer.Write(n.CreatedById, NpgsqlDbType.Text);
                writer.Write(n.UpdatedBy, NpgsqlDbType.Text);
                writer.Write(n.UpdatedById, NpgsqlDbType.Text);
                writer.Write(n.UpdateReason, NpgsqlDbType.Text);
                writer.Write(n.OwnerId, NpgsqlDbType.Text);

                writer.Write(n.IsActive, NpgsqlDbType.Boolean);
                writer.Write(n.IsDeleted, NpgsqlDbType.Boolean);
                writer.Write(n.IsApproved, NpgsqlDbType.Boolean);
                writer.Write(n.ApproverId, NpgsqlDbType.Text);
                writer.Write(DateTime.SpecifyKind(n.ApprovedDateTime ?? DateTime.UtcNow, DateTimeKind.Unspecified), NpgsqlDbType.Timestamp);

                writer.Write(n.IsAuthorized, NpgsqlDbType.Boolean);
                writer.Write(n.AuthorizedById, NpgsqlDbType.Text);
                writer.Write(DateTime.SpecifyKind(n.AuthorizedDateTime ?? DateTime.UtcNow, DateTimeKind.Unspecified), NpgsqlDbType.Timestamp);

                writer.Write(n.SysData, NpgsqlDbType.Text);
                writer.Write(n.TenantId, NpgsqlDbType.Text);
                writer.Write(n.AssociatedUserId, NpgsqlDbType.Text);
                writer.Write(n.SubTenantId, NpgsqlDbType.Text);

                writer.Write(JsonConvert.SerializeObject(n.CustomFields ?? new List<CustomField>()), NpgsqlDbType.Jsonb);
            }

            await writer.CompleteAsync();
        }

        public async Task BulkInsertZeptoMailRequestsAsync(IEnumerable<ZeptoMail> notifications)
        {
            var connectionString = _configuration["ConnectionStrings:AzureEventDBPostgreSqlDBConnection"];

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using var writer = connection.BeginBinaryImport(@"
COPY ""ZeptoMailRequest""(
    ""Id"",
    ""ApiKey"",
    ""SenderEmail"",
    ""SenderName"",
    ""To"",
    ""Cc"",
    ""Bcc"",
    ""Subject"",
    ""HtmlBody"",
    ""TextBody"",
    ""Attachments"",
    ""EventStoreId"",
    ""NotificationErrorMessage"",
    ""NotificationDelivery"",
    ""NotificationResponseDateTime"",
    ""CreatedBy"",
    ""CreatedById"",
    ""CreatedDateTime"",
    ""UpdatedBy"",
    ""UpdatedById"",
    ""UpdatedDateTime"",
    ""UpdateReason"",
    ""OwnerId"",
    ""IsActive"",
    ""IsDeleted"",
    ""IsApproved"",
    ""ApproverId"",
    ""ApprovedDateTime"",
    ""IsAuthorized"",
    ""AuthorizedById"",
    ""AuthorizedDateTime"",
    ""SysData"",
    ""TenantId"",
    ""AssociatedUserId"",
    ""SubTenantId"",
    ""CustomFields""
)
FROM STDIN (FORMAT BINARY)");

            foreach (var n in notifications)
            {
                await writer.StartRowAsync();

                writer.Write(n.Id ?? Guid.NewGuid().ToString(), NpgsqlDbType.Varchar);
                writer.Write(n.ApiKey, NpgsqlDbType.Text);
                writer.Write(n.SenderEmail, NpgsqlDbType.Varchar);
                writer.Write(n.SenderName, NpgsqlDbType.Text);

                writer.Write(JsonConvert.SerializeObject(n.To ?? new List<ZeptoMailRecipient>()), NpgsqlDbType.Jsonb);
                writer.Write(JsonConvert.SerializeObject(n.Cc ?? new List<ZeptoMailRecipient>()), NpgsqlDbType.Jsonb);
                writer.Write(JsonConvert.SerializeObject(n.Bcc ?? new List<ZeptoMailRecipient>()), NpgsqlDbType.Jsonb);

                writer.Write(n.Subject, NpgsqlDbType.Text);
                writer.Write(n.HtmlBody, NpgsqlDbType.Text);
                writer.Write(n.TextBody, NpgsqlDbType.Text);

                writer.Write(JsonConvert.SerializeObject(n.Attachments ?? new List<ZeptoMailAttachment>()), NpgsqlDbType.Jsonb);
                writer.Write(n.EventStoreId, NpgsqlDbType.Varchar);
                writer.Write(n.NotificationErrorMessage, NpgsqlDbType.Text);
                writer.Write(JsonConvert.SerializeObject(n.NotificationDelivery ?? new NotificationStatus()), NpgsqlDbType.Jsonb);

                writer.Write(DateTime.SpecifyKind(n.NotificationResponseDateTime, DateTimeKind.Unspecified), NpgsqlDbType.Timestamp);

                writer.Write(n.CreatedBy, NpgsqlDbType.Varchar);
                writer.Write(n.CreatedById, NpgsqlDbType.Varchar);
                writer.Write(DateTime.SpecifyKind(n.CreatedDateTime ?? DateTime.UtcNow, DateTimeKind.Unspecified), NpgsqlDbType.Timestamp);

                writer.Write(n.UpdatedBy, NpgsqlDbType.Varchar);
                writer.Write(n.UpdatedById, NpgsqlDbType.Varchar);
                writer.Write(DateTime.SpecifyKind(n.UpdatedDateTime ?? DateTime.UtcNow, DateTimeKind.Unspecified), NpgsqlDbType.Timestamp);

                writer.Write(n.UpdateReason, NpgsqlDbType.Text);
                writer.Write(n.OwnerId, NpgsqlDbType.Varchar);

                writer.Write(n.IsActive, NpgsqlDbType.Boolean);
                writer.Write(n.IsDeleted, NpgsqlDbType.Boolean);
                writer.Write(n.IsApproved, NpgsqlDbType.Boolean);

                writer.Write(n.ApproverId, NpgsqlDbType.Varchar);
                writer.Write(DateTime.SpecifyKind(n.ApprovedDateTime ?? DateTime.UtcNow, DateTimeKind.Unspecified), NpgsqlDbType.Timestamp);

                writer.Write(n.IsAuthorized, NpgsqlDbType.Boolean);
                writer.Write(n.AuthorizedById, NpgsqlDbType.Varchar);
                writer.Write(DateTime.SpecifyKind(n.AuthorizedDateTime ?? DateTime.UtcNow, DateTimeKind.Unspecified), NpgsqlDbType.Timestamp);

                writer.Write(n.SysData, NpgsqlDbType.Text);
                writer.Write(n.TenantId, NpgsqlDbType.Text);
                writer.Write(n.AssociatedUserId, NpgsqlDbType.Text);
                writer.Write(n.SubTenantId, NpgsqlDbType.Text);

                writer.Write(JsonConvert.SerializeObject(n.CustomFields ?? new List<CustomField>()), NpgsqlDbType.Jsonb);
            }

            await writer.CompleteAsync();
        }


    }
}
