namespace MoonWorkRAPI.Models
{
    public class JobUserNameModel
    {
        public long JobId { set; get; }
        public string? JobName { set; get; }
        public int IsUse { set; get; }
        public string? WorkflowName { set; get; }
        public string? WorkflowBlob { set; get; }
        public string? Note { set; get; }
        public DateTime SaveDate { set; get; }
        public long UserId { set; get; }
        public string? UserName { set; get; }
    }
}
