using System.Collections.Generic;
using MessagePack;

namespace Domain.Common
{
    public class PublishEventData : PublishEventBase
    {
        public string CollectionName { get; set; }
        public List<Property> EventData { get; set; }
    }

    [MessagePackObject]
    public class Property
    {
        [Key(0)]
        public string PropertyName { get; set; }

        [Key(1)]
        public object NewValue { get; set; }

        [Key(2)]
        public object OldValue { get; set; }
    }

}