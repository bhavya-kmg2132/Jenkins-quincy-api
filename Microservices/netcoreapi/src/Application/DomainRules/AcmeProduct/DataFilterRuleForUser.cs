using Application.AcmeProduct.Queries;
using Application.Common.Interfaces;
using Application.Common.Rules.Engine.Interfaces.Specification;


namespace DomainRules.AcmeProduct
{
    public class DataFilterRuleForUser : ISpecification<AcmeProductListVm>
    {
        private ICurrentUserService _dataAccess;
        public DataFilterRuleForUser(ICurrentUserService currentUserDataAccess)
        {
            _dataAccess = currentUserDataAccess;
        }

        public bool IsSatisfiedBy(AcmeProductListVm entity)
        {

            var hasUserRole = _dataAccess.UserRoles.Contains("User");

            if (hasUserRole != true)
            {
                //Process data in entity here
                return false;
            }

            return true;
        }
    }
}
