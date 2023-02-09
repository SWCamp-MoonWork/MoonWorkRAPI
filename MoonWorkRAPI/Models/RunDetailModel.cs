namespace MoonWorkRAPI.Models
{
    public class RunDetailModel
    {
        public long RunDetailId { get; set; }
        public long RunId { get; set; }
        public DateTime? ExecuteDT { get; set; }
        public string? ResultData { get; set; }
    }
}
