namespace MoonWorkRAPI.Models
{
    public class Host_JobScheduleModel
    {
        public long? JobId { set; get; }
        public string? JobName { set; get; }
        public DateTime? ScheduleStartDT { get; set; }
        public DateTime? ScheduleEndDT { get; set; }
    }
}
