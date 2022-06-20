namespace HardwareServiceOrder.API.ViewModels
{
    /// <summary>
    ///     A single event/update for a service-order that has been received and adapted from the service-provider.
    /// </summary>
    public class ServiceEvent
    {
        [Obsolete("This is being replaced with 'ServiceStatusId', and will soon be removed.")]
        public string? Status { get; set; }

        /// <summary>
        ///     The service-order status that was associated with this service-event. In most cases, the service-order's status
        ///     was updated to this status-id when the service-event was registered in our solution.
        /// </summary>
        public int ServiceStatusId { get; set; }

        /// <summary>
        ///     The time this event was recorded in the external service-provider's system.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }
    }
}
