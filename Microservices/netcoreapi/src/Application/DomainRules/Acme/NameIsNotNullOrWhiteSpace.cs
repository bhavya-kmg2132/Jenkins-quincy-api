using Application.Common.Rules.Engine.Interfaces.Specification;


namespace DomainRules.Acme
{
    public class NameIsNotNullOrWhiteSpace : ISpecification<Domain.Entities.AcmeProduct>
    {
        public bool IsSatisfiedBy(Domain.Entities.AcmeProduct entity)
        {
            return !string.IsNullOrWhiteSpace(entity.Name);
        }
    }
}
