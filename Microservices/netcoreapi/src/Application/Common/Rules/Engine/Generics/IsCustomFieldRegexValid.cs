using System.Text.RegularExpressions;
using Application.Common.Rules.Engine.Interfaces.Specification;
using Domain.Common;

namespace Application.Common.Rules.Generics
{
    /// <summary>
    /// IsCustomFieldRegexValid
    /// </summary>
    public class IsCustomFieldRegexValid : ISpecification<CustomField>
    {
        private string regexPattern;

        public IsCustomFieldRegexValid(string pattern)
        {
            regexPattern = pattern;
        }

        public bool IsSatisfiedBy(CustomField field)
        {
            Regex regex = new Regex(regexPattern);
            return regex.IsMatch(field.field_value);
        }
    }
}
