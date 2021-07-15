using System;

namespace Common.Models
{
    /// <summary>
    /// Mock model of a log object for asset events/changes
    /// </summary>
    public class AssetAuditLog
    {
        public AssetAuditLog()
        {

        }

        public AssetAuditLog(DateTime createDate, string createdBy, string message, string eventType, string lifecycleStatusBefore, string lifecycleStatusAfter)
        {
            CreatedDate = createDate;
            CreatedBy = createdBy;
            Message = message;
            EventType = eventType;
            LifecycleStatusBefore = lifecycleStatusBefore;
            LifecycleStatusAfter = lifecycleStatusAfter;
        }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Message { get; set; }
        public string EventType { get; set; }
        public string LifecycleStatusBefore {get; set;}
        public string LifecycleStatusAfter {get; set;}
    }
}
