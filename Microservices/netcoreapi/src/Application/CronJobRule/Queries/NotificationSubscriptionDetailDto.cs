using Application.Common.Mappings;
using Domain.Entities;

namespace Application.CronJobRule.Queries
{
    public class NotificationSubscriptionDetailDto : IMapFrom<NotificationSubscriptionDetail>
    {
        public string NotificationId { get; set; }
        public bool OptOut { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NotificationSubscriptionDetail, NotificationSubscriptionDetailDto>();
        }
    }
}
