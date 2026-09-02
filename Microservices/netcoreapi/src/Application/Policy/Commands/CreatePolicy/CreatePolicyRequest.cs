using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Rules.Engine.Execution;
using Application.Common.Utilities;
using Application.Policy.Rules;
using Domain.Common;
using Domain.Entities;
using Domain.Events;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PolicyEntity = Domain.Entities.Policy;

namespace Application.Policy.Commands.CreatePolicy
{
    public class CreatePolicyRequest : IRequest<string>
    {
        // ── Core ──────────────────────────────────────────────────────────────────
        public string PolicyNumber { get; set; }
        public string PolicyName { get; set; }
        public string PolicyType { get; set; }   // Marine | Cargo | Aviation
        public string StatusCode { get; set; }   // Active | Pending | Cancelled | Expired | Lapsed
        public string TransactionType { get; set; }   // NewBusiness | Renewal | Endorsement | Cancellation
        public string QuoteId { get; set; }
        public string RenewalStatus { get; set; }

        // ── Insured ───────────────────────────────────────────────────────────────
        public string InsuredId { get; set; }
        public string InsuredName { get; set; }
        public string InsuredAddress { get; set; }

        // ── Dates ─────────────────────────────────────────────────────────────────
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime? OriginalEffectiveDate { get; set; }
        public DateTime? AccountingDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string CancelReasonDescription { get; set; }

        // ── Financial ─────────────────────────────────────────────────────────────
        public decimal? TotalPremium { get; set; }
        public decimal? SumInsured { get; set; }
        public decimal? Deductible { get; set; }
        public string Currency { get; set; }

        // ── Parties ───────────────────────────────────────────────────────────────
        public string ProducerCode { get; set; }
        public string ProducerName { get; set; }
        public string UnderwriterId { get; set; }
        public string UnderwriterName { get; set; }
        public string AgentCode { get; set; }

        // ── MCA-specific ──────────────────────────────────────────────────────────
        public string VesselName { get; set; }
        public string VesselType { get; set; }
        public string CargoType { get; set; }
        public string RouteFrom { get; set; }
        public string RouteTo { get; set; }
        public string AircraftRegistration { get; set; }
        public string FlightNumber { get; set; }
        public string RiskDescription { get; set; }
        public string SurveyorName { get; set; }
        public string Remarks { get; set; }

        // ── Custom fields ─────────────────────────────────────────────────────────
        public List<CustomField> CustomFields { get; set; }
        public string CustomFieldJson { get; set; }
    }

    public class CreatePolicyRequestHandler : IRequestHandler<CreatePolicyRequest, string>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRuleEngine _ruleEngine;
        private readonly IPolicyDataAccess _policyDataAccess;
        private readonly IFieldPermissionService _fieldPermissions;

        public CreatePolicyRequestHandler(IConfiguration configuration, ILogger logger,
            IPolicyDataAccess policyDataAccess, ICurrentUserService currentUserService,
            IRuleEngine ruleEngine, IFieldPermissionService fieldPermissions)
        {
            _configuration = configuration;
            _logger = logger;
            _policyDataAccess = policyDataAccess;
            _currentUserService = currentUserService;
            _ruleEngine = ruleEngine;
            _fieldPermissions = fieldPermissions;
        }

        public async Task<string> Handle(CreatePolicyRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information: In Process
            _logger.LogInformation("CreatePolicyRequest.Handle - In Process");

            //2. Map flat request to entity
            var newPolicy = new PolicyEntity();
            newPolicy.PolicyNumber = request.PolicyNumber;
            newPolicy.PolicyName = request.PolicyName;
            newPolicy.LineOfBusinessCode = "MCA";                  // always MCA
            newPolicy.PolicyType = request.PolicyType;
            newPolicy.StatusCode = request.StatusCode;
            newPolicy.TransactionType = request.TransactionType;
            newPolicy.QuoteId = request.QuoteId;
            newPolicy.RenewalStatus = request.RenewalStatus;

            newPolicy.InsuredId = request.InsuredId;
            newPolicy.InsuredName = request.InsuredName;
            newPolicy.InsuredAddress = request.InsuredAddress;

            newPolicy.EffectiveDate = request.EffectiveDate;
            newPolicy.ExpirationDate = request.ExpirationDate;
            newPolicy.OriginalEffectiveDate = request.OriginalEffectiveDate;
            newPolicy.AccountingDate = request.AccountingDate;
            newPolicy.CancellationDate = request.CancellationDate;
            newPolicy.CancelReasonDescription = request.CancelReasonDescription;

            newPolicy.TotalPremium = request.TotalPremium;
            newPolicy.SumInsured = request.SumInsured;
            newPolicy.Deductible = request.Deductible;
            newPolicy.Currency = request.Currency;

            newPolicy.ProducerCode = request.ProducerCode;
            newPolicy.ProducerName = request.ProducerName;
            newPolicy.UnderwriterId = request.UnderwriterId;
            newPolicy.UnderwriterName = request.UnderwriterName;
            newPolicy.AgentCode = request.AgentCode;

            newPolicy.VesselName = request.VesselName;
            newPolicy.VesselType = request.VesselType;
            newPolicy.CargoType = request.CargoType;
            newPolicy.RouteFrom = request.RouteFrom;
            newPolicy.RouteTo = request.RouteTo;
            newPolicy.AircraftRegistration = request.AircraftRegistration;
            newPolicy.FlightNumber = request.FlightNumber;
            newPolicy.RiskDescription = request.RiskDescription;
            newPolicy.SurveyorName = request.SurveyorName;
            newPolicy.Remarks = request.Remarks;

            newPolicy.CustomFields = request.CustomFields;
            newPolicy.CustomFieldJson = request.CustomFieldJson;

            //4. Rule Engine
            var result = await _ruleEngine.Run(newPolicy, _configuration["RuleEngine:Policy"], "Policy");
            var failedRules = Utils.Transform(result);
            if (failedRules.Any())
            {
                Application.Common.Exceptions.ValidationException validationException =
                    new Application.Common.Exceptions.ValidationException(failedRules);
                throw validationException;
            }

            //5. Domain rules
            RuleExecutionResult ruleExecutionResult = new IsPolicyValid().Execute(newPolicy, true);

            //5.1 Custom fields validation
            if (newPolicy.CustomFields != null)
            {
                //5.1.1 Retrieve reference custom fields from the database
                var referenceCustomFields = await _policyDataAccess.GetReferenceCustomFields("McaPolicy");
                List<CustomField> referenceCustomFieldListFromDatabase = referenceCustomFields.CustomFields;

                //5.1.2 Extract and merge custom fields for insert operation
                var customFieldsDict = newPolicy.CustomFields.ToDictionary(f => f.field_name, f => (string)f.field_value?.ToString());
                newPolicy.CustomFields = Helper.ExtractCustomFieldForInsertOperation(customFieldsDict, referenceCustomFieldListFromDatabase);

                #region Custom fields domain validations
                List<RuleExecutionResult> ruleExecutionResultList = new List<RuleExecutionResult>();
                List<ValidationFailure> failures = new List<ValidationFailure>();

                foreach (var field in newPolicy.CustomFields)
                {
                    ruleExecutionResult = new IsPolicyCustomFieldsValid(field).Execute(field, false);
                    if (!ruleExecutionResult.IsValid)
                    {
                        ruleExecutionResultList.Add(ruleExecutionResult);
                        failures.Add(new ValidationFailure(field.field_name, ruleExecutionResult.Message, field.field_value?.ToString()));
                    }
                }

                if (failures.Any())
                {
                    Application.Common.Exceptions.ValidationException validationException =
                        new Application.Common.Exceptions.ValidationException(failures);
                    throw validationException;
                }
                #endregion Custom fields domain validations
            }

            //5. Audit — set after validations
            newPolicy.CorrelationId = _currentUserService.CorrelationId;
            newPolicy.AuditableRequestId = _currentUserService.RequestId;
            newPolicy.AuditableRequestName = nameof(CreatePolicyRequest);
            newPolicy.CreatedDateTime = DateTime.UtcNow;
            newPolicy.CreatedBy = _currentUserService.UserName;
            newPolicy.CreatedById = _currentUserService.UserId;

            //6. Domain events
            newPolicy.DomainEvents.Add(new PolicyCreatedEvent(newPolicy));
            newPolicy.DomainEvents.Add(new UserActivityEvent(new NetAuth.Contract.DataContract.Requests.AddUserActivity { LastActivityModule = "Policy", LastActionType = "Insert", LastActivityDetail = "Create MCA Policy", IsUserLogout = false, UserId = _currentUserService.UserId, CreatedBy = _currentUserService.UserId }));

            //7. Persist
            await _policyDataAccess.Add(newPolicy);

            //8. Logging Information: Completed
            _logger.LogInformation("CreatePolicyRequest.Handle - Completed");

            //9. Return generated Policy Id
            return newPolicy.Id;
        }
    }
}
