using Application.Common.Rules.Engine.Interfaces.Specification;

namespace DomainRules.Policy
{
    public class ExpirationDateIsAfterEffectiveDate : ISpecification<Domain.Entities.Policy>
    {
        public bool IsSatisfiedBy(Domain.Entities.Policy entity)
        {
            return entity.ExpirationDate > entity.EffectiveDate;
        }
    }
}
