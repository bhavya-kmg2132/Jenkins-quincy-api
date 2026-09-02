using Application.Common.Rules.Engine.Execution;

namespace Application.Common.Rules.Engine.Interfaces.Execution
{
    public interface IExecutor<in TEntity>
    {
        RuleExecutionResult Execute(TEntity entity);
    }
}
