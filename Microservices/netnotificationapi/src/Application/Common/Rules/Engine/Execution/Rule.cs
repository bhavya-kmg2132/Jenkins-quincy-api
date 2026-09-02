using Application.Common.Rules.Engine.Interfaces.Execution;
using Application.Common.Rules.Engine.Interfaces.Specification;

namespace Application.Common.Rules.Execution
{
    public class Rule<TEntity> : IRule<TEntity>
    {
        private readonly ISpecification<TEntity> _specification;

        public string Message { get; }

        public Rule(ISpecification<TEntity> spec, string errorMessage)
        {
            _specification = spec;
            Message = errorMessage;
        }

        public bool Execute(TEntity entity) => _specification.IsSatisfiedBy(entity);
    }
}
