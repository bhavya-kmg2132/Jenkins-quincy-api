using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Rules.Engine.Execution;
using Application.Common.Utilities;
using Domain.Common;
using Domain.Entities;
using Domain.Events;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.AcmeProduct.Commands.UpdateAcmeProduct
{
    /// <summary>
    /// class UpdateAcmeProductRequest extends the IRequest interface of MediatR
    /// </summary>
    public class UpdateAcmeProductRequest : IRequest<Unit>
    {
        public string Id { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string ProductType { get; set; }
        public decimal BaseCost { get; set; }
        public decimal BasePrice { get; set; }
        public string Currency { get; set; }
        public string ProductOwnerId { get; set; }
        public string ProductOwner { get; set; }
        public string ProductCode { get; set; }
        public bool? ProductActive { get; set; }
        public DateTime? SalesStartDate { get; set; }
        public DateTime? SalesEndDate { get; set; }
        public DateTime? SupportStartDate { get; set; }
        public DateTime? SupportEndDate { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductCategory { get; set; }
        public string ManufacturerId { get; set; }
        public string Manufacturer { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? CommissionRate { get; set; }
        public string TaxId { get; set; }
        public string Tax { get; set; }
        public string UsageUnitId { get; set; }
        public string UsageUnit { get; set; }
        public decimal? QuantityInStock { get; set; }
        public string HandlerId { get; set; }
        public string Handler { get; set; }
        public decimal? QuantityOrdered { get; set; }
        public decimal? ReorderLevel { get; set; }
        public decimal? QuantityInDemand { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }
    }

    /// <summary>
    /// For Creating handler for the above request, created UpdateAcmeProductRequestHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class UpdateAcmeProductRequestHandler : IRequestHandler<UpdateAcmeProductRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRuleEngine _ruleEngine;
        private readonly IAcmeDataAccess _acmeProductDataAccess;

        /// <summary>
        ///  Instantiates of UpdateAcmeProductRequestHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public UpdateAcmeProductRequestHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IAcmeDataAccess acmeProductDataAccess, ICurrentUserService currentUserService, IRuleEngine ruleEngine)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._acmeProductDataAccess = acmeProductDataAccess;
            this._currentUserService = currentUserService;
            this._mapper = mapper;
            this._ruleEngine = ruleEngine;
        }

        /// <summary>
        /// Handler will recieve request, process it and will return the response.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Unit</returns>
        public async Task<Unit> Handle(UpdateAcmeProductRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information In Process
            _logger.LogInformation("UpdateAcmeProductRequest.Handle - In process");

            //2. Find the request id for update in database
            var originalEntity = await _acmeProductDataAccess.GetAcmeProductById(request.Id);

            //3. if the entity not found then throw NotFoundException
            if (originalEntity == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.AcmeProduct), request.Id);
            }

            //4. Deep copy existing object before overriding it with new values
            var newEntity = (Domain.Entities.AcmeProduct)Helper.CloneObject(originalEntity);

            //5. Update entities with new values recieved in request object.
            newEntity.Id = request.Id;
            newEntity.SKU = request.SKU;
            newEntity.Name = request.Name;
            newEntity.DisplayName = request.DisplayName;
            newEntity.ShortName = request.ShortName;
            newEntity.Description = request.Description;
            newEntity.ProductType = request.ProductType;
            newEntity.BaseCost = request.BaseCost;
            newEntity.BasePrice = request.BasePrice;
            newEntity.Currency = request.Currency;
            newEntity.ProductOwnerId = request.ProductOwnerId;
            newEntity.ProductOwner = request.ProductOwner;
            newEntity.ProductCode = request.ProductCode;
            newEntity.ProductActive = request.ProductActive;
            newEntity.VendorId = request.VendorId;
            newEntity.VendorName = request.VendorName;
            newEntity.ProductCategoryId = request.ProductCategoryId;
            newEntity.ProductCategory = request.ProductCategory;
            newEntity.ManufacturerId = request.ManufacturerId;
            newEntity.Manufacturer = request.Manufacturer;
            newEntity.SalesStartDate = request.SalesStartDate;
            newEntity.SalesEndDate = request.SalesEndDate;
            newEntity.SupportStartDate = request.SupportStartDate;
            newEntity.SupportEndDate = request.SupportEndDate;
            newEntity.UnitPrice = request.UnitPrice;
            newEntity.CommissionRate = request.CommissionRate;
            newEntity.TaxId = request.TaxId;
            newEntity.Tax = request.Tax;
            newEntity.UsageUnitId = request.UsageUnitId;
            newEntity.UsageUnit = request.UsageUnit;
            newEntity.QuantityInStock = request.QuantityInStock;
            newEntity.HandlerId = request.HandlerId;
            newEntity.Handler = request.Handler;
            newEntity.QuantityOrdered = request.QuantityOrdered;
            newEntity.ReorderLevel = request.ReorderLevel;
            newEntity.QuantityInDemand = request.QuantityInDemand;

            newEntity.UpdatedBy = _currentUserService.UserId;
            newEntity.UpdatedDateTime = DateTime.UtcNow;

            //6. Rule Engine  
            var existingProducts = await _acmeProductDataAccess.GetAcmeProductList();
            var existingNames = existingProducts?
                .Where(p => p != null && p.Id != newEntity.Id && !string.IsNullOrWhiteSpace(p.Name))
                .Select(p => p.Name.Trim())
                .ToList() ?? new List<string>();

            var result = await _ruleEngine.Run(new object[] { newEntity, existingNames }, _configuration[$"RuleEngine:AcmeProduct"], "AcmeProduct");

            var failedRules = Utils.Transform(result);
            // Raise error if any failures found
            if (failedRules.Any())
            {
                Application.Common.Exceptions.ValidationException validationException = new Application.Common.Exceptions.ValidationException(failedRules);
                throw validationException;
            }

            //7. Extract Custom Field For Update Operation 
            if (request.CustomFields != null)
            {
                var customFieldResult = Helper.ExtractCustomFieldForUpdateOperation(request.CustomFields, newEntity.CustomFields);
                newEntity.CustomFields = customFieldResult;

                #region Custom fields domain validations
                List<RuleExecutionResult> ruleExecutionResultList = new List<RuleExecutionResult>();
                List<ValidationFailure> failures = new List<ValidationFailure>();

                // Execute domain rules
                RuleExecutionResult ruleExecutionResultForAcmeValid = new Application.Rules.AcmeProduct.IsAcmeValid(_acmeProductDataAccess).Execute(newEntity, true);

                // Execute domain rules
                foreach (var field in newEntity.CustomFields)
                {
                    RuleExecutionResult ruleExecutionResult = new Application.Rules.Acme.IsAcmeCustomFieldsValid(field, _acmeProductDataAccess).Execute(field, false);
                    if (!ruleExecutionResult.IsValid)
                    {
                        //Manage rule execution results
                        ruleExecutionResultList.Add(ruleExecutionResult);

                        //Manage validations
                        failures.Add(new ValidationFailure(field.field_name, ruleExecutionResult.Message, field.field_value));
                    }
                }

                // Raise error if any failures found
                if (failures.Any())
                {
                    Application.Common.Exceptions.ValidationException validationException = new Application.Common.Exceptions.ValidationException(failures);
                    throw validationException;
                }

                #endregion Custom fields domain validations
            }

            //8. Initiate CorrelationIdCreator to entity to start new chain of events, which is required in pipeline and event processing
            newEntity.CorrelationId = _currentUserService.CorrelationId;
            newEntity.AuditableRequestId = _currentUserService.RequestId;
            newEntity.AuditableRequestName = nameof(UpdateAcmeProductRequest);
            newEntity.UpdatedDateTime = System.DateTime.UtcNow;
            newEntity.UpdatedBy = _currentUserService.UserName;
            newEntity.UpdatedById = _currentUserService.UserId;

            //9. Adding Acme object to domain event.
            newEntity.DomainEvents.Add(new AcmeProductUpdatedEvent(newEntity, originalEntity));

            // Add UserActivityEvent to track user activity This event will be handled by UserActivityEventHandler to log user activity
            newEntity.DomainEvents.Add(new UserActivityEvent(new NetAuth.Contract.DataContract.Requests.AddUserActivity { LastActivityModule = "AcmeProduct", LastActionType = "Update", LastActivityDetail = "Update AcmeProduct", IsUserLogout = false, UserId = _currentUserService.UserId, CreatedBy = _currentUserService.UserId }));

            //10. Update the Acmeproduct
            await _acmeProductDataAccess.Update(newEntity);

            //11. Logging Information Completed
            _logger.LogInformation("UpdateAcmeProductRequest.Handle - Completed");

            //12. Return unit
            return Unit.Value;
        }
    }
}
