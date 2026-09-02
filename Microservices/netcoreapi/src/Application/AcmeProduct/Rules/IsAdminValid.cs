using Application.AcmeProduct.Queries;
using Application.Common.Interfaces;
using Application.Common.Rules.Engine.Execution;
using Application.Common.Rules.Execution;
using DomainRules.AcmeProduct;

namespace Application.Rules.AcmeProduct
{
    public class IsAdminValid : Executor<Application.AcmeProduct.Queries.AcmeProductListVm>
    {
        public IsAdminValid(ICurrentUserService currentUserService)
        {
            //Using standard
            Add("DataFilterRuleForAdmin", new Rule<AcmeProductListVm>(new DataFilterRuleForAdmin(currentUserService), "User is not valid"));
        }
    }
}
