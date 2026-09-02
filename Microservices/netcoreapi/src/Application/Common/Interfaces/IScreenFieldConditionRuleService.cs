using System.Collections.Generic;
using System.Threading.Tasks;
using Application.ExternalPolicy.Models;
using Application.ExternalPolicy.Rules;

namespace Application.Common.Interfaces
{
    /// <summary>
    /// Runs the Screen Field Conditions rule engine workflows
    /// (RuleEngine/ScreenFieldConditions/ScreenFieldConditionRules.json) against the DB2
    /// policy payloads and throws <see cref="Application.Common.Exceptions.ValidationException"/>
    /// when any rule fails.
    /// </summary>
    public interface IScreenFieldConditionRuleService
    {
        Task Validate(IEnumerable<PolicyDataTable> policyData);

        // For handlers whose request is already strongly-typed (not a raw PolicyDataTable
        // payload) - the caller builds the context via ScreenRuleRequestMapper.
        Task Validate(ScreenRuleEvaluationContext context);
    }
}
