using System;
using Application.Common.Rules.Engine.Interfaces.Specification;
using Domain.Common;

namespace Application.Common.Rules.Generics
{

    public class IsCustomFieldDatatypeValid : ISpecification<CustomField>
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

            Object value = new Object();

            switch (entity.field_type)
            {
                case "boolean":
                    if (!bool.TryParse(entity.field_value, out bool boolValue))
                    {
                        return false; // Invalid boolean value
                    }
                    value = boolValue;
                    break;

                case "text":
                    if (!(entity.field_value is string) && entity.field_value != null)
                    {
                        return false;
                    }
                    value = entity.field_value; // No further validation for text
                    break;

                case "datetime":
                    if (!DateTime.TryParse(entity.field_value, out DateTime dateTimeValue))
                    {
                        return false; // Invalid datetime value
                    }
                    value = dateTimeValue;
                    break;

                case "number":
                    if (!double.TryParse(entity.field_value, out double doubleValue))
                    {
                        return false; // Invalid number value
                    }
                    value = doubleValue;
                    break;

                default:
                    return false; // Unsupported field type
            }

            // Return true if the value satisfies the criteria
            return true;
        }
    }

}
