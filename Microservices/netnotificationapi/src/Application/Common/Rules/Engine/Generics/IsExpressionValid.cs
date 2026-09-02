using System;
using System.Linq.Expressions;
using Application.Common.Rules.Engine.Interfaces.Specification;

namespace Application.Common.Rules.Generics
{
    public class IsExpressionValid<TEntity> : ISpecification<TEntity>
    {
        private readonly Expression<Func<TEntity, bool>> _expression;

        public IsExpressionValid(Expression<Func<TEntity, bool>> expression)
        {
            _expression = expression;
        }

        public bool IsSatisfiedBy(TEntity entity) => _expression.Compile()(entity);
    }
}
