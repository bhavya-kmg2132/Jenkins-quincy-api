using Application.AcmeProduct.Queries;
using Application.Common.Interfaces;
using Application.Common.Rules.Engine.Interfaces.Specification;

namespace DomainRules.AcmeProduct
{
    public class DataFilterRuleForAdmin : ISpecification<AcmeProductListVm>
    {
        private ICurrentUserService _dataAccess;
        public DataFilterRuleForAdmin(ICurrentUserService userDataAccess)
        {
            _dataAccess = userDataAccess;
        }

        public bool IsSatisfiedBy(AcmeProductListVm entity)
        {
            var hasAdminRole = _dataAccess.UserRoles.Contains("Admin");

            //Process data in entity here
            if (!hasAdminRole)
            {
                //Process data in entity here
                return false;
            }

            return true;
        }
    }
}
