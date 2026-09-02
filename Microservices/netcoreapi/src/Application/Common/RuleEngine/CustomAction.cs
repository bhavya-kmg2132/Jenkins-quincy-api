using System;
using System.Threading.Tasks;
using RulesEngine.Actions;
using RulesEngine.Models;

namespace Application.Common.RuleEngine
{
    public class CustomAction : ActionBase
    {
        public override ValueTask<object> Run(ActionContext context, RuleParameter[] ruleParameters)
        {
            if (context.TryGetContext<string>("Message", out var message) && !string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine(message);
                return new ValueTask<object>(message);
            }

            const string defaultMsg = "Custom action executed.";
            Console.WriteLine(defaultMsg);
            return new ValueTask<object>(defaultMsg);
        }
    }
}
