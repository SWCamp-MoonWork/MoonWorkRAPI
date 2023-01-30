namespace MoonWorkRAPI.Models
{
    public class Job_HostRunModel
    {
        public string? HostName { get; set; }
        public string? HostIp { get; set; }
        public DateTime? StartDT { get; set; }
        public DateTime? EndDT { get; set; }
        public string? State { get; set; }
        public DateTime? SaveDate { get; set; }
    }
}
