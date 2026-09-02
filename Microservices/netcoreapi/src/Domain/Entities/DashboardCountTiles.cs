namespace Domain.Entities
{
    public class DashboardCountTiles
    {
        public long OutputSchemaCount { get; set; }
        public long InputSchemaCount { get; set; }
        public long ApprovalPending { get; set; }
        public long Schedules { get; set; }
    }
}
