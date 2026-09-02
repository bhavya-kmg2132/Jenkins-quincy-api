namespace Application.Common.Rules.Engine.Interfaces.Execution
{
    public interface IRule<in TEntity>
    {
        string Message { get; }

        bool Execute(TEntity entity);
    }
}
