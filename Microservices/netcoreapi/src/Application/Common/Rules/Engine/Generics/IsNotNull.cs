using System;
using System.Linq.Expressions;
using Application.Common.Rules.Engine.Interfaces.Specification;

namespace Application.Common.Rules.Generics
{
    public class IsNotNull<TEntity> : ISpecification<TEntity>
    {
        private readonly Expression<Func<TEntity, object>> _expression;

        public IsNotNull(Expression<Func<TEntity, object>> expression)
        {
            _expression = expression;
        }

        public bool IsSatisfiedBy(TEntity entity)
        {
            var value = _expression.Compile()(entity);
            var type = typeof(TEntity);

            if (type.Equals(typeof(string)))
                return !string.IsNullOrEmpty(value as string);
            else if (type.Equals(typeof(int)))
                return ((int)value) > 0;
            else if (type.Equals(typeof(DateTime)))
                return !((DateTime)value).Equals(DateTime.MinValue);

            return value != null;
        }
    }
}
