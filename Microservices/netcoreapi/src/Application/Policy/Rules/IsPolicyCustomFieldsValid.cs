using Application.Common.Rules.Engine.Execution;
using Application.Common.Rules.Execution;
using Application.Common.Rules.Generics;
using Domain.Common;

namespace Application.Policy.Rules
{
    public class IsPolicyCustomFieldsValid : Executor<CustomField>
    {
        public IsPolicyCustomFieldsValid(CustomField field)
        {
            if (field.field_is_required)
            {
                Add("IsNotNullOrEmpty",
                    new Rule<CustomField>(
                        new IsCustomFieldRequiredValid(),
                        $"{field.field_name} cannot be null or empty."));
            }

            Add("IsLengthValid",
                new Rule<CustomField>(
                    new IsCustomFieldLengthValid(),
                    $"{field.field_name} cannot be more than {field.field_length}."));

            Add("IsDatatypeValid",
                new Rule<CustomField>(
                    new IsCustomFieldDatatypeValid(),
                    $"Type mismatch for '{field.field_name}': value '{field.field_value}' is not of type '{field.field_type}'."));

            if (!string.IsNullOrEmpty(field.regex))
            {
                Add("IsRegexValid",
                    new Rule<CustomField>(
                        new IsCustomFieldRegexValid(field.regex),
                        $"{field.field_name} does not match the required pattern."));
            }
        }
    }
}
