using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Notification.Commands.SendEmailNotification
{
    public class MarkAsReadRequest : IRequest<string>
    {
        public string UserId { get; set; }
        public string NotificationId { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created MarkAsReadRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class MarkAsReadRequestHandler : IRequestHandler<MarkAsReadRequest, string>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailNotificationService _EmailService;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _ienvironment;

        /// <summary>
        /// Instantiates the MarkAsReadRequestHandler class
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="mapper"></param>
        /// <param name="emailService"></param>
        /// <param name="environment"></param>
        public MarkAsReadRequestHandler(ILogger logger, IConfiguration configuration, IMapper mapper, IEmailNotificationService emailService, IWebHostEnvironment environment)
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
        public async Task<string> Handle(MarkAsReadRequest request, CancellationToken cancellationToken)
        {

            //1. Logging Information - In process
            _logger.LogInformation("MarkAsReadRequest.Handle - In process");

            //2. Asynchronously send an email 
            var response = await _EmailService.MarkAsRead(request.UserId, request.NotificationId);

            //3. Logging Information - Completed
            _logger.LogInformation("MarkAsReadRequest.Handle - Completed");

            //7. Return response
            return response;
        }
    }
}
