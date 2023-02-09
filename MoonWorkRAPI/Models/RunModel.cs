namespace MoonWorkRAPI.Models
{
    public class RunModel
    {
        public long? RunId { get; set; }
        public string? WorkflowName {get; set;}
        public DateTime? StartDT { get; set; }
        public DateTime? EndDT { get; set; }
        public string? State { get; set; }
        public long? JobId { get; set; }
        public long? HostId { get; set; }
        public DateTime? SaveDate { get; set; }
        public string? ResultData { get; set; }
        public string? Duration { get; set; }
    }
}