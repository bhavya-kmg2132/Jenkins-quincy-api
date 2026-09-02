using Application.Common.Rules.Engine.Execution;
using Application.Common.Rules.Execution;
using DomainRules.Policy;

namespace Application.Policy.Rules
{
    public class IsPolicyDeletable : Executor<Domain.Entities.Policy>
    {
        public IsPolicyDeletable()
        {
            Add("PolicyIsEligibleForDeletion",
                new Rule<Domain.Entities.Policy>(
                    new PolicyIsEligibleForDeletion(),
                    "A Cancelled policy cannot be deleted"));
        }
    }
}
