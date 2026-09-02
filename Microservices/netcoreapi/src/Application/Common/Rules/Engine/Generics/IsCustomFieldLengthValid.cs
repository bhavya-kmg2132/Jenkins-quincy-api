using System;
using Application.Common.Rules.Engine.Interfaces.Specification;
using Domain.Common;

namespace Application.Common.Rules.Generics
{

    public class IsCustomFieldLengthValid : ISpecification<CustomField>
    {
        public bool IsSatisfiedBy(CustomField entity)
        {
            if (entity.field_type.ToLower() == "text")
            {
                if (!string.IsNullOrEmpty(entity.field_value))
                {
                    string local_field_value = Convert.ToString(entity.field_value);
                    if (local_field_value.Length > entity.field_length)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
