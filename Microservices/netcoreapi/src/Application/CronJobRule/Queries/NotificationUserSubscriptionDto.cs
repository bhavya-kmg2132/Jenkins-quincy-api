using System.Collections.Generic;
using Application.Common.Mappings;

namespace Application.CronJobRule.Queries
{
    public class NotificationUserSubscriptionDto : IMapFrom<NotificationUserSubscription>
    {
        public string UserId { get; set; }
        public List<NotificationSubscriptionDetailDto> SubscriptionDetails { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NotificationUserSubscription, NotificationUserSubscriptionDto>();
        }
    }
}
