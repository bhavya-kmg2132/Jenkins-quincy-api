using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.ZeptoMail.Commands.SendTransactionalEmail
{
    public class SendTransactionalEmailRequest : IRequest<string>
    {

        public string ApiKey { get; set; }
        //public ZeptoMailAddress From { get; set; }
        public List<ZeptoMailRecipient> To { get; set; } = new();
        public List<ZeptoMailRecipient>? Cc { get; set; }
        public List<ZeptoMailRecipient>? Bcc { get; set; }
        public string Subject { get; set; }
        public string? HtmlBody { get; set; }
        public string? TextBody { get; set; }
        public List<ZeptoMailAttachment>? Attachments { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created SendTransactionalMailRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class SendTransactionalMailRequestHandler : IRequestHandler<SendTransactionalEmailRequest, string>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IZeptoMailService _zeptoMailService;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _ienvironment;

        /// <summary>
        /// Instantiates the SendTransactionalMailRequestHandler class
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="mapper"></param>
        /// <param name="emailService"></param>
        /// <param name="environment"></param>
        public SendTransactionalMailRequestHandler(ILogger logger, IConfiguration configuration, IMapper mapper, IZeptoMailService emailService, IWebHostEnvironment environment)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._zeptoMailService = emailService;
            this._mapper = mapper;
            this._ienvironment = environment;
        }

        /// <summary>
        /// Handler will receive request, process it and will return the response. 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>string</returns>
        public async Task<string> Handle(SendTransactionalEmailRequest query, CancellationToken cancellationToken)
        {

            //1. Logging Information - In process
            _logger.LogInformation("SendTransactionalEmailRequest.Handle - In process");

            //2. Add entity value in Notification class
            var entity = new Domain.Entities.ZeptoMail();

            entity.ApiKey = query.ApiKey;
            entity.To = query.To;
            entity.Cc = query.Cc;
            entity.Bcc = query.Bcc;
            entity.Subject = query.Subject;
            entity.HtmlBody = query.HtmlBody;
            entity.Attachments = query.Attachments;

            //3. Asynchronously send an email 
            var notificationResponse = await _zeptoMailService.SendEmailAsync(entity);

            //4. Add Domain Events
            entity.DomainEvents.Add(new ZeptoMailCreatedEvent(notificationResponse));

            //5. Dispatch event for notification
            await _zeptoMailService.DispatchEvents(entity);

            //6. Logging Information - Completed
            _logger.LogInformation("SendTransactionalEmailRequest.Handle - Completed");

            var response = notificationResponse.NotificationDelivery.DeliveryReport;

            //7. Return response
            return response;
        }
    }
}
