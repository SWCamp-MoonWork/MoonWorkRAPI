namespace MoonWorkRAPI.Models
{
    public class Job_UserScheduleModel
    {
        public long? JobId { get; set; }
        public string? JobName { set; get; }
        public Boolean? JobIsUse { set; get; }
        public string? WorkflowName { set; get; }
        public DateTime? JobSaveDate { set; get; }
        public String? JobNote { set; get; }
        public string? UserName { set; get; }
        public long? ScheduleId { set; get; }
        public String? ScheduleName { set; get; }
        public Boolean? ScheduleIsUse { get; set; }
        public Boolean? ScheduleType { set; get; }
        public DateTime? OneTimeOccurDT { set; get; }
        public string? CronExpression { set; get; }
        public DateTime? ScheduleStartDT { set; get; }
        public DateTime? ScheduleEndDT { set; get; }
        public DateTime? ScheduleSaveDate { set; get; }
    }
}
