using Application.Common.Rules.Engine.Interfaces.Specification;
using Domain.Common;

namespace Application.Common.Rules.Generics
{

    public class IsCustomFieldRequiredValid : ISpecification<CustomField>
    {
        public bool IsSatisfiedBy(CustomField entity)
        {
            if (entity.field_is_required)
            {
                if (string.IsNullOrEmpty(entity.field_value))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
