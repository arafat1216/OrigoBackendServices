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

        public AssetAuditLog(Guid assetAuditLogId, Guid assetId, Guid customerId, DateTime createDate, string createdBy, string message, string eventType, string lifecycleStatusBefore, string lifecycleStatusAfter)
        {
            AssetAuditLogId = assetAuditLogId;
            AssetId = assetId;
            CustomerId = customerId;
            CreatedDate = createDate;
            CreatedBy = createdBy;
            Message = message;
            EventType = eventType;
            LifecycleStatusBefore = lifecycleStatusBefore;
            LifecycleStatusAfter = lifecycleStatusAfter;
        }

        /// <summary>
        /// External Id of this AssetAuditLog
        /// </summary>
        public Guid AssetAuditLogId { get; set; }

        /// <summary>
        /// External Id of the Asset this AssetAuditLog pertains to.
        /// </summary>
        public Guid AssetId { get; set; }

        /// <summary>
        /// The customer the user who caused the event belongs to. May not be the same customer as the user who owns the asset.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The date the change on the asset occured.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Name/Identifier of person who caused the change on asset.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Event description and effect.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Type of event (registration, change user, etc)
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// The lifecycle of the asset before the event.
        /// Not all events will change the lifecycle.
        /// </summary>
        public string LifecycleStatusBefore {get; set;}

        /// <summary>
        /// The lifecycle of the asset after the event.
        /// Not all events will change the lifecycle.
        /// </summary>
        public string LifecycleStatusAfter {get; set;}
    }
}
