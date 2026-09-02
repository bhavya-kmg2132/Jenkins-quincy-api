using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.AcmeProduct.Commands.DeleteAcmeProduct
{
    /// <summary>
    /// class DeleteAcmeProductRequest extends the IRequest interface of MediatR
    /// </summary>
    public class DeleteAcmeProductRequest : IRequest<Unit>
    {
        public string Id { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created DeleteAcmeProductRequest class
    /// that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class DeleteAcmeProductRequestHandler : IRequestHandler<DeleteAcmeProductRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IAcmeDataAccess _dataAccess;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Instantiates DeleteAcmeRequestHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public DeleteAcmeProductRequestHandler(IConfiguration configuration, ILogger logger, IAcmeDataAccess acmeProductDataAccess, ICurrentUserService currentUserService)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = acmeProductDataAccess;
            this._currentUserService = currentUserService;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Unit</returns>
        public async Task<Unit> Handle(DeleteAcmeProductRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information In Process
            _logger.LogInformation("DeleteAcmeProductRequest.Handle - In process");

            //2. Find the requested UserId to delete in Database.
            var entity = await _dataAccess.GetAcmeProductById(request.Id);

            //3. If entity does not exist throw exception not found.
            if (entity == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.AcmeProduct), request.Id);
            }

            //4. Add values to auditable properties for the entity 
            entity.UpdatedDateTime = System.DateTime.UtcNow;
            entity.UpdatedBy = _currentUserService.UserId;

            //5. Initiate CorrelationIdCreator to entity to start new chain of events, which is required in pipeline and event processing
            entity.CorrelationId = _currentUserService.CorrelationId;
            entity.AuditableRequestId = _currentUserService.RequestId;
            entity.AuditableRequestName = nameof(DeleteAcmeProductRequest);
            entity.UpdatedDateTime = System.DateTime.UtcNow;
            entity.UpdatedBy = _currentUserService.UserName;
            entity.UpdatedById = _currentUserService.UserId;

            //6. Adding Acme object to domain event.
            entity.DomainEvents.Add(new AcmeProductDeletedEvent(entity));

            // Add UserActivityEvent to track user activity This event will be handled by UserActivityEventHandler to log user activity
            entity.DomainEvents.Add(new UserActivityEvent(new NetAuth.Contract.DataContract.Requests.AddUserActivity { LastActivityModule = "AcmeProduct", LastActionType = "Delete", LastActivityDetail = "Delete AcmeProduct", IsUserLogout = false, UserId = _currentUserService.UserId, CreatedBy = _currentUserService.UserId }));

            //7. Delete the Acme with requested Id
            await _dataAccess.Delete(entity);

            //8. Logging Information Completed
            _logger.LogInformation("DeleteAcmeProductRequest.Handle - Completed");

            //9. Return Unit
            return Unit.Value;
        }
    }
}
