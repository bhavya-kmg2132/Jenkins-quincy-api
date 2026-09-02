using System.Collections.Generic;

namespace Domain.Common
{
    public class PublishEventData : PublishEventBase
    {
        public string CollectionName { get; set; }
        public List<Property> Data { get; set; }
    }

    public class Property
    {
        public string PropertyName { get; set; }
        public object NewValue { get; set; }
        public object OldValue { get; set; }
    }

}