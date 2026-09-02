using Application.AcmeProduct.Queries;
using Application.Common.Interfaces;
using Application.Common.Rules.Engine.Interfaces.Specification;


namespace DomainRules.AcmeProduct
{
    public class DataFilterRuleForPower : ISpecification<AcmeProductListVm>
    {
        private ICurrentUserService _dataAccess;
        public DataFilterRuleForPower(ICurrentUserService userDataAccess)
        {
            _dataAccess = userDataAccess;
        }

        public bool IsSatisfiedBy(AcmeProductListVm entity)
        {
            var hasPowerRole = _dataAccess.UserRoles.Contains("Power");

            if (!hasPowerRole)
            {
                //Process data in entity here
                return false;
            }

            return true;
        }
    }
}
