using System;
using Application.Common.Mappings;

namespace Application.CronJobRule.Queries
{
    public class CronJobRuleDto : IMapFrom<Domain.Entities.CronJobRule>
    {
        public string NotificationName { get; set; }
        public string Id { get; set; }
        public DateTime LastExecutionDate { get; set; }
        public string Frequency { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public string Role { get; set; }
        public string ExecutionDay { get; set; }
        public string ExecutionMonth { get; set; }
        public bool IsNotificationPaused { get; set; } = true;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.CronJobRule, CronJobRuleDto>();

        }
    }
}