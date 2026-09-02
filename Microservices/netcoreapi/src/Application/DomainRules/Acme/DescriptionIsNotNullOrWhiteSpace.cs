using Application.Common.Rules.Engine.Interfaces.Specification;


namespace DomainRules.Acme
{
    public class DescriptionIsNotNullOrWhiteSpace : ISpecification<Domain.Entities.AcmeProduct>
    {
        public bool IsSatisfiedBy(Domain.Entities.AcmeProduct entity)
        {
            return !string.IsNullOrWhiteSpace(entity.Description);
        }
    }
}
