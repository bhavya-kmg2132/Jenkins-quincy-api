using System.Collections.Generic;

namespace Application.PublishEvent.Queries
{
    public class PublishEventDataListVm
    {
        public IList<PublishEventDataDto> PublishEventData { get; set; }
        public int TotalCount { get; set; }
    }
}
