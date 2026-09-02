using System.Collections.Generic;

namespace Domain.Common
{
    public class CustomField
    {
        public string field_name { get; set; }
        public string display_name { get; set; }
        public string div_id { get; set; }
        public dynamic field_value { get; set; }
        public string field_type { get; set; }
        public int field_length { get; set; }
        public bool field_is_required { get; set; }
        public string field_defaultValue { get; set; }
        public string regex { get; set; }

    }
    public class ReferenceCustomFields
    {
        public List<CustomField> CustomFields { get; set; }
        public string TableName { get; set; }
        public string _id { get; set; }
    }

}
