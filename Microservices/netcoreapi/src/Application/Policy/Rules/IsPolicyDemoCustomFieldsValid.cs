using Application.Common.Rules.Engine.Execution;
using Application.Common.Rules.Execution;
using Application.Common.Rules.Generics;
using Domain.Common;


namespace Application.Rules.PolicyDemo
{
    public class IsPolicyDemoCustomFieldsValid : Executor<CustomField>
    {
        public IsPolicyDemoCustomFieldsValid(CustomField field)
        {
            if (field.field_is_required)
            {
                Add("IsNotNullOrEmpty", new Rule<CustomField>(new IsCustomFieldRequiredValid(), $"{field.field_name} cannot be null or empty."));
            }
            Add("IsLengthValid", new Rule<CustomField>(new IsCustomFieldLengthValid(), $"{field.field_name} cannot be more than {field.field_length}."));
            Add("IsDatatypeValid", new Rule<CustomField>(new IsCustomFieldDatatypeValid(), $"Type mismatch exception for custom field '{field.field_name}' : Data value recieved '{field.field_value}', is not of '{field.field_type}' data type."));
        }
    }
}

