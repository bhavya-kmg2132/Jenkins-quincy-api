using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.SystemDate.Queries
{
    /// <summary>
    /// class GetSystemDateQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetSystemDateQuery : IRequest<DateTime>
    {
        public DateTime SystemDate { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request , created GetSystemDateQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetSystemDateQueryHandler : IRequestHandler<GetSystemDateQuery, DateTime>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Instantiates GetSystemDateQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public GetSystemDateQueryHandler(IConfiguration configuration, ILogger logger)
        {
            this._configuration = configuration;
            this._logger = logger;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<DateTime> Handle(GetSystemDateQuery request, CancellationToken cancellationToken)
        {
            request.SystemDate = System.DateTime.Now;
            await Task.CompletedTask;
            return request.SystemDate;
        }
    }
}
