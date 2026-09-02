using Application.Common.Rules.Engine.Interfaces.Specification;

namespace DomainRules.Policy
{
    public class InsuredNameIsRequired : ISpecification<Domain.Entities.Policy>
    {
        public bool IsSatisfiedBy(Domain.Entities.Policy entity)
        {
            return !string.IsNullOrWhiteSpace(entity.InsuredName);
        }
    }
}
