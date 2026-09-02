using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Rules.Engine.Execution;
using Application.Common.Utilities;
using Domain.Common;
using Domain.Entities;
using Domain.Events;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.AcmeProduct.Commands.CreateAcmeProduct
{
    /// <summary>
    /// class CreateAcmeRequest extends the IRequest interface of MediatR
    /// </summary>
    public class CreateAcmeProductRequest : IRequest<string>
    {
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
    /// For Creating handler for the above request, created CreateAcmeProductRequest class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class CreateAcmeProductRequestHandler : IRequestHandler<CreateAcmeProductRequest, string>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRuleEngine _ruleEngine;
        private readonly IAcmeDataAccess _dataAccess;

        /// <summary>
        /// Instantiates the class CreateAcmeProductRequestHandler
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="dataAccess"></param>
        /// <param name="currentUserService"></param>
        public CreateAcmeProductRequestHandler(IConfiguration configuration, ILogger logger, IAcmeDataAccess dataAccess, ICurrentUserService currentUserService, IRuleEngine ruleEngine)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            this._currentUserService = currentUserService;
            this._ruleEngine = ruleEngine;
        }

        /// <summary>
        /// Handler will recieve request, process it and will return the response.  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>string</returns>
        public async Task<string> Handle(CreateAcmeProductRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information: In Process
            _logger.LogInformation("CreateAcmeProductRequest.Handle - In Process");

            //2. Assign requested Acme values 
            var acmeProduct = new Domain.Entities.AcmeProduct();
            acmeProduct.SKU = request.SKU;
            acmeProduct.Name = request.Name;
            acmeProduct.Name = request.Name;
            acmeProduct.DisplayName = request.DisplayName;
            acmeProduct.ShortName = request.ShortName;
            acmeProduct.Description = request.Description;
            acmeProduct.ProductType = request.ProductType;
            acmeProduct.BaseCost = request.BaseCost;
            acmeProduct.BasePrice = request.BasePrice;
            acmeProduct.Currency = request.Currency;
            acmeProduct.ProductOwnerId = request.ProductOwnerId;
            acmeProduct.ProductOwner = request.ProductOwner;
            acmeProduct.ProductCode = request.ProductCode;
            acmeProduct.ProductActive = request.ProductActive;
            acmeProduct.VendorId = request.VendorId;
            acmeProduct.VendorName = request.VendorName;
            acmeProduct.ProductCategoryId = request.ProductCategoryId;
            acmeProduct.ProductCategory = request.ProductCategory;
            acmeProduct.ManufacturerId = request.ManufacturerId;
            acmeProduct.Manufacturer = request.Manufacturer;
            acmeProduct.SalesStartDate = request.SalesStartDate;
            acmeProduct.SalesEndDate = request.SalesEndDate;
            acmeProduct.SupportStartDate = request.SupportStartDate;
            acmeProduct.SupportEndDate = request.SupportEndDate;
            acmeProduct.UnitPrice = request.UnitPrice;
            acmeProduct.CommissionRate = request.CommissionRate;
            acmeProduct.TaxId = request.TaxId;
            acmeProduct.Tax = request.Tax;
            acmeProduct.UsageUnitId = request.UsageUnitId;
            acmeProduct.UsageUnit = request.UsageUnit;
            acmeProduct.QuantityInStock = request.QuantityInStock;
            acmeProduct.HandlerId = request.HandlerId;
            acmeProduct.Handler = request.Handler;
            acmeProduct.QuantityOrdered = request.QuantityOrdered;
            acmeProduct.ReorderLevel = request.ReorderLevel;
            acmeProduct.QuantityInDemand = request.QuantityInDemand;

            acmeProduct.CreatedDateTime = System.DateTime.UtcNow;
            acmeProduct.CreatedBy = _currentUserService.UserId;

            //3. Rule Engine  
            // Fetch existing product names and pass them as second input to rules engine
            var existingProducts = await _dataAccess.GetAcmeProductList();
            var existingNames = existingProducts?
                                    .Where(p => !string.IsNullOrEmpty(p.Name))
                                    .Select(p => p.Name.Trim())
                                    .ToList()
                                ?? new List<string>();

            // Pass both inputs as an object[] so RuleEngine expands them to input1, input2, ...
            var result = await _ruleEngine.Run(new object[] { acmeProduct, existingNames }, _configuration[$"RuleEngine:AcmeProduct"], "AcmeProduct");

            var failedRules = Utils.Transform(result);
            // Raise error if any failures found
            if (failedRules.Any())
            {
                Application.Common.Exceptions.ValidationException validationException = new Application.Common.Exceptions.ValidationException(failedRules);
                throw validationException;
            }

            //4. Execute domain rules
            RuleExecutionResult ruleExecutionResult = new Application.Rules.AcmeProduct.IsAcmeValid(_dataAccess).Execute(acmeProduct, true);
            if (request.CustomFields != null)
            {
                //4.1 Retrieve reference custom fields from the database 
                var referenceCustomFields = await _dataAccess.GetReferenceCustomFields("AcmeProduct");
                List<CustomField> referenceCustomFieldListFromDatabase = referenceCustomFields.CustomFields;

                //4.2 Extract Custom Field For Insert Operation
                var customFieldResult = Helper.ExtractCustomFieldForInsertOperation(request.CustomFields, referenceCustomFieldListFromDatabase);
                acmeProduct.CustomFields = customFieldResult;

                #region Custom fields domain validations
                List<RuleExecutionResult> ruleExecutionResultList = new List<RuleExecutionResult>();
                List<ValidationFailure> failures = new List<ValidationFailure>();

                foreach (var field in acmeProduct.CustomFields)
                {
                    ruleExecutionResult = new Application.Rules.Acme.IsAcmeCustomFieldsValid(field, _dataAccess).Execute(field, false);
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

            //5. Initiate CorrelationIdCreator to entity to start new chain of events, which is required in pipeline and event processing
            acmeProduct.CorrelationId = _currentUserService.CorrelationId;
            acmeProduct.AuditableRequestId = _currentUserService.RequestId;
            acmeProduct.AuditableRequestName = nameof(CreateAcmeProductRequest);
            acmeProduct.CreatedDateTime = System.DateTime.UtcNow;
            acmeProduct.CreatedBy = _currentUserService.UserName;
            acmeProduct.CreatedById = _currentUserService.UserId;

            //6. Adding Acme object to domain event.
            acmeProduct.DomainEvents.Add(new AcmeProductCreatedEvent(acmeProduct));

            // Add UserActivityEvent to track user activity This event will be handled by UserActivityEventHandler to log user activity
            acmeProduct.DomainEvents.Add(new UserActivityEvent(new NetAuth.Contract.DataContract.Requests.AddUserActivity { LastActivityModule = "AcmeProduct", LastActionType = "Insert", LastActivityDetail = "Create AcmeProduct", IsUserLogout = false, UserId = _currentUserService.UserId, CreatedBy = _currentUserService.UserId }));

            //7. Add requested value to Acme
            await _dataAccess.Add(acmeProduct);

            //8. Logging Information Completed
            _logger.LogInformation("CreateAcmeProductRequest.Handle - Completed");

            //9. Return generated Acme id
            return acmeProduct.Id;
        }
    }
}
