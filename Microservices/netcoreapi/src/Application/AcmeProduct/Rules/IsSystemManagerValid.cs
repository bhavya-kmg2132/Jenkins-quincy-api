using Application.Common.Interfaces;
using Application.Common.Rules.Engine.Execution;
using Application.Common.Rules.Execution;
using DomainRules.AcmeProduct;

namespace Application.Rules.AcmeProduct
{
    public class IsSystemManagerValid : Executor<Application.AcmeProduct.Queries.AcmeProductListVm>
    {
        public IsSystemManagerValid(ICurrentUserService currentUserService)
        {
            //Using standard
            Add("DataFilterRuleForSystemManager", new Rule<Application.AcmeProduct.Queries.AcmeProductListVm>(new DataFilterRuleForSystemManager(currentUserService), "System Manager is not valid"));
        }
    }
}
