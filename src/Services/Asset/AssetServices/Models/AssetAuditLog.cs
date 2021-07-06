using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AssetServices.Models
{
    /// <summary>
    /// Mock model of a log object for asset events/changes
    /// </summary>
    public class AssetAuditLog
    {
        public AssetAuditLog(DateTime createDate, string createdBy, string message, string eventType, string lifecycleStatusBefore, string lifecycleStatusAfter)
        {
            CreatedDate = createDate;
            CreatedBy = createdBy;
            Message = message;
            EventType = eventType;
            LifecycleStatusBefore = lifecycleStatusBefore;
            LifecycleStatusAfter = lifecycleStatusAfter;
        }

        [JsonProperty]
        DateTime CreatedDate { get; set; }
        [JsonProperty]
        string CreatedBy { get; set; }
        [JsonProperty]
        string Message { get; set; }
        [JsonProperty]
        string EventType { get; set; }
        [JsonProperty]
        string LifecycleStatusBefore {get; set;}
        [JsonProperty]
        string LifecycleStatusAfter {get; set;}

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
