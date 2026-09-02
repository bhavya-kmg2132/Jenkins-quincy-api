using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Dto;

namespace Application.Common.Interfaces
{
    public interface IRuleEngine
    {
        Task<List<RuleEngineResult>> Run(object input, string jsonFile, string workflowName);
    }
}
