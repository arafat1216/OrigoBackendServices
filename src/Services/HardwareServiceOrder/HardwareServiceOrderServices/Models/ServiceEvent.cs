using System.ComponentModel.DataAnnotations.Schema;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     A single event/update for a service-order that has been received and adapted from the service-provider.
    /// </summary>
    public class ServiceEvent
    {
        public int Id { get; set; }

        /// <summary>
        ///     The <see cref="ServiceStatus.Id"/> that is associated with the service-event.
        /// </summary>
        /// <see cref="ServiceStatusEnum"/>
        public int ServiceStatusId { get; set; }

        /// <summary>
        ///     The time this event was recorded in the external service-provider's system.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }
    }
}
