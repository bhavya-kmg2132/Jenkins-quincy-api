using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
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

namespace Application.Policy.Commands.UpdatePolicy
{
    public class UpdatePolicyRequest : IRequest<Unit>
    {
        // ── Identity (required to locate the record) ──────────────────────────────
        public string Id { get; set; }

        // ── Core ──────────────────────────────────────────────────────────────────
        // PolicyNumber is intentionally omitted — it is immutable and preserved from the DB record.
        public string PolicyName { get; set; }
        public string PolicyType { get; set; }   // Marine | Cargo | Aviation

        public string StatusCode { get; set; }   // Active | Pending | Cancelled | Expired | Lapsed

        public string TransactionType { get; set; }   // NewBusiness | Renewal | Endorsement | Cancellation
        public string QuoteId { get; set; }

        [FieldPermission(view: FieldPermission.Core_Policy_RenewalStatus_View, edit: FieldPermission.Core_Policy_RenewalStatus_Edit, throwError: false)]
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
        [FieldPermission(view: FieldPermission.Core_Policy_TotalPremium_View, edit: FieldPermission.Core_Policy_TotalPremium_Edit, throwError: false)]
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

    public class UpdatePolicyRequestHandler : IRequestHandler<UpdatePolicyRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRuleEngine _ruleEngine;
        private readonly IPolicyDataAccess _policyDataAccess;
        private readonly IFieldPermissionService _fieldPermissions;

        public UpdatePolicyRequestHandler(IConfiguration configuration, ILogger logger,
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

        public async Task<Unit> Handle(UpdatePolicyRequest request, CancellationToken cancellationToken)
        {
            //1. Logging Information: In Process
            _logger.LogInformation("UpdatePolicyRequest.Handle - In Process");

            //2. Find the request id for update in database
            //needed for field-permission comparison and to preserve immutable fields
            var originalEntity = await _policyDataAccess.GetPolicyById(request.Id);

            //3. if the entity not found then throw NotFoundException
            if (originalEntity == null)
                throw new NotFoundException(nameof(PolicyEntity), request.Id);

            //4. Deep copy existing object before overriding it with new values
            var policy = (PolicyEntity)Helper.CloneObject(originalEntity);

            //5. Update entities with new values recieved in request object.
            policy.Id = request.Id;
            policy.PolicyNumber = originalEntity.PolicyNumber;  // immutable — always kept from DB
            policy.LineOfBusinessCode = "MCA";                        // always MCA

            policy.PolicyName = request.PolicyName;
            policy.PolicyType = request.PolicyType;
            policy.StatusCode = request.StatusCode;
            policy.TransactionType = request.TransactionType;
            policy.QuoteId = request.QuoteId;
            policy.RenewalStatus = request.RenewalStatus;

            policy.InsuredId = request.InsuredId;
            policy.InsuredName = request.InsuredName;
            policy.InsuredAddress = request.InsuredAddress;

            policy.EffectiveDate = request.EffectiveDate;
            policy.ExpirationDate = request.ExpirationDate;
            policy.OriginalEffectiveDate = request.OriginalEffectiveDate;
            policy.AccountingDate = request.AccountingDate;
            policy.CancellationDate = request.CancellationDate;
            policy.CancelReasonDescription = request.CancelReasonDescription;

            policy.TotalPremium = request.TotalPremium;
            policy.SumInsured = request.SumInsured;
            policy.Deductible = request.Deductible;
            policy.Currency = request.Currency;

            policy.ProducerCode = request.ProducerCode;
            policy.ProducerName = request.ProducerName;
            policy.UnderwriterId = request.UnderwriterId;
            policy.UnderwriterName = request.UnderwriterName;
            policy.AgentCode = request.AgentCode;

            policy.VesselName = request.VesselName;
            policy.VesselType = request.VesselType;
            policy.CargoType = request.CargoType;
            policy.RouteFrom = request.RouteFrom;
            policy.RouteTo = request.RouteTo;
            policy.AircraftRegistration = request.AircraftRegistration;
            policy.FlightNumber = request.FlightNumber;
            policy.RiskDescription = request.RiskDescription;
            policy.SurveyorName = request.SurveyorName;
            policy.Remarks = request.Remarks;

            policy.CustomFields = request.CustomFields;
            policy.CustomFieldJson = request.CustomFieldJson;

            //4. Handler-level field-permission enforcement
            // Compares original (DB) vs proposed (policy) for every [FieldPermission(edit:...)]
            // property on the entity. If the user changed a restricted field without the required
            // edit permission, throwError=false reverts it silently to the original DB value.
            await _fieldPermissions.ApplyEditPermissionsAsync(originalEntity, policy);

            //5. Rule Engine
            var result = await _ruleEngine.Run(policy, _configuration["RuleEngine:Policy"], "Policy");
            var failedRules = Utils.Transform(result);
            if (failedRules.Any())
            {
                Application.Common.Exceptions.ValidationException validationException =
                    new Application.Common.Exceptions.ValidationException(failedRules);
                throw validationException;
            }

            //6. Domain rules
            RuleExecutionResult ruleExecutionResult = new IsPolicyValid().Execute(policy, true);

            //6.1 Custom fields validation
            if (policy.CustomFields != null)
            {
                //6.1.1 Retrieve reference custom fields from the database
                var referenceCustomFields = await _policyDataAccess.GetReferenceCustomFields("McaPolicy");
                List<CustomField> referenceCustomFieldListFromDatabase = referenceCustomFields.CustomFields;

                //6.1.2 Extract and merge custom fields for update operation
                var customFieldsDict = policy.CustomFields.ToDictionary(f => f.field_name, f => (string)f.field_value?.ToString());
                policy.CustomFields = Helper.ExtractCustomFieldForUpdateOperation(customFieldsDict, referenceCustomFieldListFromDatabase);

                #region Custom fields domain validations
                List<RuleExecutionResult> ruleExecutionResultList = new List<RuleExecutionResult>();
                List<ValidationFailure> failures = new List<ValidationFailure>();

                foreach (var field in policy.CustomFields)
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

            //7. Audit — set after validations
            policy.CorrelationId = _currentUserService.CorrelationId;
            policy.AuditableRequestId = _currentUserService.RequestId;
            policy.AuditableRequestName = nameof(UpdatePolicyRequest);
            policy.UpdatedDateTime = DateTime.UtcNow;
            policy.UpdatedBy = _currentUserService.UserName;
            policy.UpdatedById = _currentUserService.UserId;

            //8. Domain events
            policy.DomainEvents.Add(new PolicyUpdatedEvent(policy, originalEntity));
            policy.DomainEvents.Add(new UserActivityEvent(new NetAuth.Contract.DataContract.Requests.AddUserActivity { LastActivityModule = "Policy", LastActionType = "Update", LastActivityDetail = "Update MCA Policy", IsUserLogout = false, UserId = _currentUserService.UserId, CreatedBy = _currentUserService.UserId }));

            //9. Persist
            await _policyDataAccess.Update(policy);

            //10. Logging Information: Completed
            _logger.LogInformation("UpdatePolicyRequest.Handle - Completed");

            return Unit.Value;
        }
    }
}
