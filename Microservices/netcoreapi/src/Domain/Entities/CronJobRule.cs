using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Events;

namespace Domain.Entities
{
    public class CronJobRule : AuditableEntity, IHasDomainEvent
    {
        public string NotificationName { get; set; }
        public string Id { get; set; }
        public DateTime LastExecutionDate { get; set; }
        public string Frequency { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public string Role { get; set; }
        public string ExecutionDay { get; set; }
        public int ExecutionMonth { get; set; }

        public bool IsNotificationPaused { get; set; } = true;

        private bool _done;
        [NotMapped]
        public bool Done
        {
            get => _done;
            set
            {
                if (value == true && _done == false)
                {
                    DomainEvents.Add(new CronJobRuleCompletedEvent(this));
                }

                _done = value;
            }
        }
        [NotMapped]
        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

    }
}