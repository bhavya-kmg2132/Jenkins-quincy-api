using Application.Common.Rules.Engine.Interfaces.Specification;

namespace DomainRules.Policy
{
    public class PolicyIsEligibleForDeletion : ISpecification<Domain.Entities.Policy>
    {
        private const string CancelledStatus = "Cancelled";

        public bool IsSatisfiedBy(Domain.Entities.Policy entity)
        {
            return !string.Equals(entity.StatusCode, CancelledStatus,
                System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
