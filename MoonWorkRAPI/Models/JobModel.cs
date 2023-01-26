using com.sun.org.apache.xpath.@internal.axes;
using java.sql;

namespace MoonWorkRAPI.Models
{
    public class JobModel
    {
        public long JobId { set; get; }
        public string? JobName { set; get; }
        public int IsUse { set; get; }
        public string? WorkflowName { set; get; }
        public byte[]? WorkflowBlob { set; get; }
        public string? Note { set; get; }
        public DateTime SaveDate { set; get; }
        public long UserId { set; get; }

    }
}