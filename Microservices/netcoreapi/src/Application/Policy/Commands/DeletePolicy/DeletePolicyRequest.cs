using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Utilities;
using Application.Policy.Rules;
using Domain.Entities;
using Domain.Events;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PolicyEntity = Domain.Entities.Policy;

namespace Application.Policy.Commands.DeletePolicy
{
    public class DeletePolicyRequest : IRequest<Unit>
    {
        public PolicyEntity Policy { get; set; }
    }

    public class DeletePolicyRequestHandler : IRequestHandler<DeletePolicyRequest, Unit>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRuleEngine _ruleEngine;
        private readonly IPolicyDataAccess _policyDataAccess;

        public DeletePolicyRequestHandler(IConfiguration configuration, ILogger logger,
            IPolicyDataAccess policyDataAccess, ICurrentUserService currentUserService,
            IRuleEngine ruleEngine)
        {
            _configuration = configuration;
            _logger = logger;
            _policyDataAccess = policyDataAccess;
            _currentUserService = currentUserService;
            _ruleEngine = ruleEngine;
        }

        public async Task<Unit> Handle(DeletePolicyRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeletePolicyRequest.Handle - In Process");

            // 1. Fetch entity; guard not-found
            var policy = await _policyDataAccess.GetPolicyById(request.Policy.Id);
            if (policy == null)
                throw new NotFoundException(nameof(PolicyEntity), request.Policy.Id);

            // 2. Entity assignment — set audit fields on the fetched entity
            policy.CorrelationId = _currentUserService.CorrelationId;
            policy.AuditableRequestId = _currentUserService.RequestId;
            policy.AuditableRequestName = nameof(DeletePolicyRequest);
            policy.UpdatedDateTime = DateTime.UtcNow;
            policy.UpdatedBy = _currentUserService.UserName;
            policy.UpdatedById = _currentUserService.UserId;

            // 3. Domain rules
            var domainResult = new IsPolicyDeletable().Execute(policy);
            if (!domainResult.IsValid)
            {
                var failures = domainResult.Errors
                    .Select(e => new ValidationFailure(e.Name, e.Message))
                    .ToList();
                throw new Common.Exceptions.ValidationException(failures);
            }

            // 4. Rule engine
            var ruleResult = await _ruleEngine.Run(policy, _configuration["RuleEngine:Policy"], "PolicyDelete");
            var failedRules = Utils.Transform(ruleResult);
            if (failedRules.Any())
                throw new Common.Exceptions.ValidationException(failedRules);

            // 5. Domain events
            policy.DomainEvents.Add(new PolicyDeletedEvent(policy));
            policy.DomainEvents.Add(new UserActivityEvent(new NetAuth.Contract.DataContract.Requests.AddUserActivity { LastActivityModule = "Policy", LastActionType = "Delete", LastActivityDetail = "Delete MCA Policy", IsUserLogout = false, UserId = _currentUserService.UserId, CreatedBy = _currentUserService.UserId }));

            await _policyDataAccess.Delete(policy);

            _logger.LogInformation("DeletePolicyRequest.Handle - Completed");
            return Unit.Value;
        }
    }
}
