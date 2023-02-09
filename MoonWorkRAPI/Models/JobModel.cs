using com.sun.org.apache.xpath.@internal.axes;
using java.sql;

namespace MoonWorkRAPI.Models
{
    public class JobModel
    {
        public long? JobId { set; get; }
        public string? JobName { set; get; }
        public Boolean? IsUse { set; get; }
        public string? WorkflowName { set; get; }
        public string? WorkflowBlob { set; get; }
        public string? Note { set; get; }
        public DateTime? SaveDate { set; get; }
        public long? UserId { set; get; }
        public string? State { set; get; }
    }
}