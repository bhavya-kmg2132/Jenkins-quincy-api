using System;
using System.Collections.Generic;
using Application.Common.Rules.Engine.Interfaces.Specification;

namespace DomainRules.Policy
{
    public class PolicyTypeIsValid : ISpecification<Domain.Entities.Policy>
    {
        private static readonly HashSet<string> _validTypes =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Marine", "Cargo", "Aviation" };

        public bool IsSatisfiedBy(Domain.Entities.Policy entity)
        {
            return !string.IsNullOrWhiteSpace(entity.PolicyType)
                && _validTypes.Contains(entity.PolicyType);
        }
    }
}
