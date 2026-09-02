using System;

namespace Domain.Enums
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class StringValueAttribute : Attribute
    {
        public string Value { get; }

        public StringValueAttribute(string value)
        {
            Value = value;
        }
    }

    public enum CustomFieldsDatatype
    {
        [StringValue("number")]
        number = 1,

        [StringValue("text")]
        text = 2,

        [StringValue("datetime")]
        datetime = 3,

        [StringValue("boolean")]
        boolean = 4
    }
}
