using Application.Common.Rules.Engine.Interfaces.Specification;

namespace DomainRules.Acme
{
    public class IsDescriptionLengthValid : ISpecification<Domain.Entities.AcmeProduct>
    {
        public bool IsSatisfiedBy(Domain.Entities.AcmeProduct entity)
        {
            return entity.Description.Length >= 11 /*&& long.TryParse(entity.Description, out long i)*/;
        }
    }
}
