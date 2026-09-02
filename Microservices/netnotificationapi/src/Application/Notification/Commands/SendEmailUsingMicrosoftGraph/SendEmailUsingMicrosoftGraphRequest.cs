using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Notification.Commands.SendEmailUsingMicrosoftGraph
{
    public class SendEmailUsingMicrosoftGraphRequest : IRequest<string>
    {
        public string Id { get; set; }
        public string ApiKey { get; set; }
        public string notification_type { get; set; }
        public string email_to { get; set; }
        public List<string> email_toName { get; set; }
        public string email_cc { get; set; }
        public List<string> email_ccName { get; set; }
        public string email_bcc { get; set; }
        public List<string> email_bccName { get; set; }
        public string email_subject { get; set; }
        public string email_body { get; set; }
        public DateTime NotificationRequestDateTime { get; set; }
        public List<EmailAttachment> email_attachments { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created SendEmailUsingMicrosoftGraphRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class SendEmailUsingMicrosoftGraphRequestHandler : IRequestHandler<SendEmailUsingMicrosoftGraphRequest, string>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailNotificationService _EmailService;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _ienvironment;

        /// <summary>
        /// Instantiates the SendEmailUsingMicrosoftGraphRequestHandler class
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="mapper"></param>
        /// <param name="emailService"></param>
        /// <param name="environment"></param>
        public SendEmailUsingMicrosoftGraphRequestHandler(ILogger logger, IConfiguration configuration, IMapper mapper, IEmailNotificationService emailService, IWebHostEnvironment environment)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._EmailService = emailService;
            this._mapper = mapper;
            this._ienvironment = environment;
        }

        /// <summary>
        /// Handler will receive request, process it and will return the response. 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>string</returns>
        public async Task<string> Handle(SendEmailUsingMicrosoftGraphRequest query, CancellationToken cancellationToken)
        {

            //1. Logging Information - In process
            _logger.LogInformation("SendEmailUsingMicrosoftGraphRequest.Handle - In process");

            //2. Add entity value in Notification class
            var entity = new Domain.Entities.PostgreNotification();

            entity.Id = query.Id;
            entity.ApiKey = query.ApiKey;
            entity.EntityJson.NotificationType = query.notification_type;
            entity.EmailTo = query.email_to;
            entity.EntityJson.EmailToName = query.email_toName;
            entity.EmailCc = query.email_cc;
            entity.EntityJson.EmailCcName = query.email_ccName;
            entity.EmailBcc = query.email_bcc;
            entity.EntityJson.EmailBccName = query.email_bccName;
            entity.EmailSubject = query.email_subject;
            entity.EmailBody = query.email_body;
            entity.EntityJson.NotificationRequestDateTime = query.NotificationRequestDateTime;
            entity.EmailAttachments = query.email_attachments;

            //3. Asynchronously send an email 
            var notificationResponse = await _EmailService.SendEmailNotificationUsingMicrosoftGraph(entity);

            //4. Add Domain Events
            entity.DomainEvents.Add(new NotificationCreatedEvent(notificationResponse));

            //5. Dispatch event for notification
            await _EmailService.DispatchEvents(entity);

            //6. Logging Information - Completed
            _logger.LogInformation("SendEmailUsingMicrosoftGraphRequest.Handle - Completed");

            var response = notificationResponse.NotificationDelivery.DeliveryReport;

            //7. Return response
            return response;
        }
    }
}
