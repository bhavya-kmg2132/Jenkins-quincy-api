using Application.Common.Interfaces;
using Application.Common.Rules.Engine.Execution;
using Application.Common.Rules.Execution;
using Application.Common.Rules.Generics;
using Domain.Common;


namespace Application.Rules.Acme
{
    public class IsAcmeCustomFieldsValid : Executor<CustomField>
    {
        public IsAcmeCustomFieldsValid(CustomField field, IAcmeDataAccess acmeDataAccess)
        {
            if (field.field_is_required)
            {
                Add("IsNotNullOrEmpty", new Rule<CustomField>(new IsCustomFieldRequiredValid(), $"{field.field_name} cannot be null or empty."));
            }

            Add("IsLengthValid", new Rule<CustomField>(new IsCustomFieldLengthValid(), $"{field.field_name} cannot be more than {field.field_length}."));
            Add("IsDatatypeValid", new Rule<CustomField>(new IsCustomFieldDatatypeValid(), $"Type mismatch exception for custom field '{field.field_name}' : Data value recieved '{field.field_value}', is not of '{field.field_type}' data type."));

            // Add regex validation rule
            if (!string.IsNullOrEmpty(field.regex))
            {
                Add("IsRegexValid", new Rule<CustomField>(new IsCustomFieldRegexValid(field.regex), $"{field.field_name} does not match the required pattern."));
            }
        }
    }
}

