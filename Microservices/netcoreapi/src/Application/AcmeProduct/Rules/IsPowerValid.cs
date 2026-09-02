using Application.Common.Interfaces;
using Application.Common.Rules.Engine.Execution;
using Application.Common.Rules.Execution;
using DomainRules.AcmeProduct;

namespace Application.Rules.AcmeProduct
{
    public class IsPowerValid : Executor<Application.AcmeProduct.Queries.AcmeProductListVm>
    {
        public IsPowerValid(ICurrentUserService currentUserService)
        {
            //Using standard
            Add("DataFilterRuleForPower", new Rule<Application.AcmeProduct.Queries.AcmeProductListVm>(new DataFilterRuleForPower(currentUserService), "Power is not valid"));
        }
    }
}
