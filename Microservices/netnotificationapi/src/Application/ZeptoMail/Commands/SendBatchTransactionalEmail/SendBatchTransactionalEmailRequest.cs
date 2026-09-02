using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.ZeptoMail.Commands.SendBatchTransactionalEmail
{
    public class SendBatchTransactionalEmailRequest : IRequest<List<Domain.Entities.ZeptoMail>>
    {
        public List<Domain.Entities.ZeptoMail> ZeptoMails { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created SendTransactionalMailRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class SendTransactionalMailRequestHandler : IRequestHandler<SendBatchTransactionalEmailRequest, List<Domain.Entities.ZeptoMail>>
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
        public async Task<List<Domain.Entities.ZeptoMail>> Handle(SendBatchTransactionalEmailRequest query, CancellationToken cancellationToken)
        {

            //1. Logging Information - In process
            _logger.LogInformation("SendBatchTransactionalEmailRequest.Handle - In process");

            //2. Asynchronously send batch emails 
            var notificationResponse = await _zeptoMailService.SendBatchEmailAsync(query.ZeptoMails);

            //6. Logging Information - Completed
            _logger.LogInformation("SendBatchTransactionalEmailRequest.Handle - Completed");

            //7. Return response
            return notificationResponse;
        }
    }
}
