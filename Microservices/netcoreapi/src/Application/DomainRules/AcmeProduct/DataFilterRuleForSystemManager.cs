using Application.AcmeProduct.Queries;
using Application.Common.Interfaces;
using Application.Common.Rules.Engine.Interfaces.Specification;


namespace DomainRules.AcmeProduct
{
    public class DataFilterRuleForSystemManager : ISpecification<AcmeProductListVm>
    {
        private ICurrentUserService _dataAccess;
        public DataFilterRuleForSystemManager(ICurrentUserService userDataAccess)
        {
            _dataAccess = userDataAccess;
        }

        public bool IsSatisfiedBy(AcmeProductListVm entity)
        {
            var hasSystemManagerRole = _dataAccess.UserRoles.Contains("System Manager");

            if (!hasSystemManagerRole)
            {
                //Process data in entity here
                return false;
            }

            return true;
        }
    }
}
