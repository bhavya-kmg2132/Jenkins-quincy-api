using Application.Common.Interfaces;
using Application.Common.Rules.Engine.Execution;
using Application.Common.Rules.Execution;
using DomainRules.AcmeProduct;

namespace Application.Rules.AcmeProduct
{
    public class IsUserValid : Executor<Application.AcmeProduct.Queries.AcmeProductListVm>
    {
        public IsUserValid(ICurrentUserService currentUserService)
        {
            //Using standard
            Add("DataFilterRuleForAdmin", new Rule<Application.AcmeProduct.Queries.AcmeProductListVm>(new DataFilterRuleForUser(currentUserService), "User is not valid"));
        }
    }
}
