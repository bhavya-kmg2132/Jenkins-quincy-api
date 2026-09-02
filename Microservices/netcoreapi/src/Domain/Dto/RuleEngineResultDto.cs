using System.Collections.Generic;

namespace Domain.Dto
{
    public class RuleEngineResult
    {
        public string RuleName { get; set; }
        public bool IsSuccess { get; set; }
        public string Outcome { get; set; }
        public List<RuleEngineResult> ChildResult { get; set; }
    }
}
