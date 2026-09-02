using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Application.Common.Models
{
    public class OrderedPropertiesView_Obsolete
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public SimpleProperty[] Properties { get; }

        public OrderedPropertiesView_Obsolete(object input)
        {
            this.Properties = input.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Select(prop => new SimpleProperty(prop, input))
                .ToArray();
        }

        [DebuggerDisplay("{Value}", Name = "{PropertyName,nq}")]
        public class SimpleProperty
        {
            public SimpleProperty(MemberInfo member, object input)
            {
                this.Value = GetValue(member, input);
                this.PropertyName = member.Name;
            }

            private object GetValue(MemberInfo member, object input)
            {
                switch (member)
                {
                    case FieldInfo fi: return fi.GetValue(input);
                    case PropertyInfo pi: return pi.GetValue(input);
                    default: return null;
                }
            }

            public object Value { get; internal set; }
            public string PropertyName { get; internal set; }
        }
    }
}
