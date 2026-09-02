using Application.Common.Interfaces;
using Application.Common.Rules.Engine.Interfaces.Specification;

namespace DomainRules.Acme
{
    public class NameIsNotDuplicate : ISpecification<Domain.Entities.AcmeProduct>
    {
        private IAcmeDataAccess _dataAccess;
        public NameIsNotDuplicate(IAcmeDataAccess acmeDataAccess)
        {
            _dataAccess = acmeDataAccess;
        }
        public bool IsSatisfiedBy(Domain.Entities.AcmeProduct entity)
        {
            var IsNameExists = _dataAccess.FindAcmeProductByName(entity.Name);

            if (IsNameExists.Result == false)
            {
                //entity.Name = "Domain rule executed at " + DateTime.UtcNow.ToString();
                return true;

            }
            else
            {
                //entity.Name = "Domain rule executed at " + DateTime.UtcNow.ToString();
                return false;
            }
            ;
        }
    }
}
