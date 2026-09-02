using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.AcmeOrders.Commands.DeletePermanentAcmeProduct
{
    //[InvalidateCache(typeof(GetTodosQuery), typeof(GetTodoItemsWithPaginationQuery))]

    /// <summary>
    /// class DeletePermanentAcmeOrderCommand extends the IRequest interface of MediatR
    /// </summary>
    public class DeletePermanentAcmeProductCommand : IRequest<Unit>
    {
        public string Id { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created DeletePermanentAcmeProductCommand class
    /// that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class DeletePermanentAcmeProductCommandHandler : IRequestHandler<DeletePermanentAcmeProductCommand, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAcmeDataAccess _dataAccess;

        /// <summary>
        /// Instantiates DeletePermanentAcmeProductCommandHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="dataAccess"></param>
        /// <param name="currentUserService"></param>
        public DeletePermanentAcmeProductCommandHandler(IConfiguration configuration, ILogger logger, IAcmeDataAccess dataAccess, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._currentUserService = currentUserService;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Unit</returns>
        public async Task<Unit> Handle(DeletePermanentAcmeProductCommand request, CancellationToken cancellationToken)
        {
            //1. Logging Information In Process
            _logger.LogInformation("DeletePermanentAcmeOrderCommand.Handle - In process");

            //2. Find the requested Id to delete in Database.
            var entity = await _dataAccess.GetAcmeProductById(request.Id);

            //3. If entity does not exist throw exception not found.
            if (entity == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.AcmeProduct), request.Id);
            }

            //7. Delete the AcmeOrder with requested Id
            await _dataAccess.DeletePermanentAcmeProduct(request.Id);

            //8. Logging Information Completed
            _logger.LogInformation("DeletePermanentAcmeProductCommand.Handle - Completed");

            //9. Return Unit
            return Unit.Value;
        }
    }
}
