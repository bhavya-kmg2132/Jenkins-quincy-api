using System;

namespace Domain.Entities
{
    public class Scheduler
    {
        public string _id { get; set; }
        public string Title { get; set; }
        public DateTime StartDateTime { get; set; }
        public string RecurringFrequency { get; set; }
        public string ProcessedFiles { get; set; }
        public DateTime? LastExecutionDate { get; set; }
        public string ExecutionDay { get; set; }
    }
}
