using Common.Seedwork;
using System.ComponentModel.DataAnnotations.Schema;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     A single event/update for a service-order that has been received and adapted from the service-provider.
    /// </summary>
    public class ServiceEvent : EntityV2
    {
        /// <summary>
        ///     The <see cref="HardwareServiceOrder"/> that is associated with the service-event.
        /// </summary>
        public int HardwareServiceOrderId { get; set; }

        /// <summary>
        ///     The <see cref="ServiceStatus.Id"/> that is associated with the service-event.
        /// </summary>
        /// <see cref="ServiceStatusEnum"/>
        public int ServiceStatusId { get; set; }

        /// <summary>
        ///     The time this event was recorded in the external service-provider's system.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }


        /*
         * EF navigation properties
         */

        /// <summary>
        ///     The <see cref="Models.HardwareServiceOrder"/> that is associated with the service-event.
        /// </summary>
        /// <remarks>
        ///     This is a Entity Framework navigation property, and may not be included in the queried results.
        /// </remarks>
        [ForeignKey(nameof(HardwareServiceOrderId))]
        public virtual HardwareServiceOrder? HardwareServiceOrder { get; set; }
    }
}
