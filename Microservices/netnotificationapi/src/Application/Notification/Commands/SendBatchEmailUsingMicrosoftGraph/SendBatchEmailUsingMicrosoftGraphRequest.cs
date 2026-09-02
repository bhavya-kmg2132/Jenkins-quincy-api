using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Notification.Commands.SendBatchEmailUsingMicrosoftGraph
{
    public class SendBatchEmailUsingMicrosoftGraphRequest : IRequest<List<Domain.Entities.PostgreNotification>>
    {
        public List<Domain.Entities.PostgreNotification> notificationList { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created SendBatchEmailUsingMicrosoftGraphRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class SendBatchEmailUsingMicrosoftGraphRequestHandler : IRequestHandler<SendBatchEmailUsingMicrosoftGraphRequest, List<Domain.Entities.PostgreNotification>>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailNotificationService _EmailService;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _ienvironment;

        /// <summary>
        /// Instantiates the SendBatchEmailUsingMicrosoftGraphRequestHandler class
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="mapper"></param>
        /// <param name="emailService"></param>
        /// <param name="environment"></param>
        public SendBatchEmailUsingMicrosoftGraphRequestHandler(ILogger logger, IConfiguration configuration, IMapper mapper, IEmailNotificationService emailService, IWebHostEnvironment environment)
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
        public async Task<List<Domain.Entities.PostgreNotification>> Handle(SendBatchEmailUsingMicrosoftGraphRequest query, CancellationToken cancellationToken)
        {

            //1. Logging Information - In process
            _logger.LogInformation("SendBatchEmailUsingMicrosoftGraphRequest.Handle - In process");

            //2. Calling the BatchEmailNotification Method.
            var notificationResponse = await _EmailService.SendBatchEmailNotificationUsingMicrosoftGraph(query.notificationList);

            //3. Logging Information - Completed
            _logger.LogInformation("SendBatchEmailUsingMicrosoftGraphRequest.Handle - Completed");

            //4. Return response
            return notificationResponse;
        }
    }
}
