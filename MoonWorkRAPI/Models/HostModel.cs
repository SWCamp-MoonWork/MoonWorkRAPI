namespace MoonWorkRAPI.Models
{
    public class HostModel
    {
        public long HostId { get; set; }
        public string? HostName { get; set; }
        public Boolean IsUse { get; set; }
        public String? Role { get; set; }
        public String? EndPoint { get; set; }
        public String? Note { get; set; }
        public DateTime SaveDate { get; set; }
        public long UserId { get; set; }
    }
}
