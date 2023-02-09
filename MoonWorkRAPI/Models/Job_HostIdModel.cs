namespace MoonWorkRAPI.Models
{
    public class Job_HostIdModel
    {
        public long? JobId { get; set; }
        public string? JobName { get; set; }
        public DateTime? LastRun { get; set; }
        public string? NextRun { get; set; }
    }
}
