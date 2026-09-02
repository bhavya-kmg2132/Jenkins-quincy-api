using Application.Common.Rules.Engine.Execution;
using Application.Common.Rules.Execution;
using DomainRules.Policy;

namespace Application.Policy.Rules
{
    public class IsPolicyValid : Executor<Domain.Entities.Policy>
    {
        public IsPolicyValid()
        {
            Add("InsuredNameIsRequired",
                new Rule<Domain.Entities.Policy>(
                    new InsuredNameIsRequired(),
                    "Insured Name is required"));

            Add("ExpirationDateIsAfterEffectiveDate",
                new Rule<Domain.Entities.Policy>(
                    new ExpirationDateIsAfterEffectiveDate(),
                    "Expiration Date must be after Effective Date"));

            Add("PolicyTypeIsValid",
                new Rule<Domain.Entities.Policy>(
                    new PolicyTypeIsValid(),
                    "Policy Type must be Marine, Cargo, or Aviation"));
        }
    }
}
