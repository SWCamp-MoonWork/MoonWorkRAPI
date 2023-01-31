namespace MoonWorkRAPI.Models
{
    public class Schedule_IsUseSelectModel
    {
        public long ScheduleId { get; set; }
        public long JobId { get; set; }
        public string? ScheduleName { get; set; }
        public Boolean? IsUse { get; set; }
        public Boolean? ScheduleType { get; set; }
        public DateTime? OneTimeOccurDT { get; set; }
        public String? CronExpression { get; set; }
        public DateTime? ScheduleStartDT { get; set; }
        public DateTime? ScheduleEndDT { get; set; }
        public DateTime? SaveDate { get; set; }
        public long UserId { get; set; }
        public Boolean? JobIsUse { get; set; }
        public string? WorkflowName { get; set; }
    }
}
