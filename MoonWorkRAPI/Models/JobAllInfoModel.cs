namespace MoonWorkRAPI.Models
{
    public class JobAllInfoModel
    {
        public long JobId { set; get; }
        public string? JobName { set; get; }
        public string? WorkflowName { set; get; }
        public string? HostIp { set; get; }
        public string? HostName { set; get; }
        public Boolean? JobIsUse { set; get; }
        public string? UserName { set; get; }
        public DateTime? JobSaveDate { set; get; }
        public String? JobNote { set; get; }
        public String? ScheduleId { set; get; }
        public String? ScheduleName { set; get; }
        public Boolean? ScheduleIsUse { get; set; }
        public Boolean? ScheduleType { set; get; }
        public DateTime? OneTimeOccurDT { set; get; }
        public DateTime? ScheduleStartDT { set; get; }
        public DateTime? ScheduleEndDT { set; get; }
        public DateTime? ScheduleSaveDate { set; get; }
    }
}
