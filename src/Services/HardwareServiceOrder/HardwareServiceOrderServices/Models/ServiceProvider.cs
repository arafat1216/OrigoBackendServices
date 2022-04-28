using Common.Seedwork;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents a single service-provider.
    /// </summary>
    public class ServiceProvider : EntityV2, IAggregateRoot
    {
        /// <summary>
        ///     The public ID for the partner's Organization entry.
        /// </summary>
        public Guid OrganizationId { get; set; }

        // TODO: Add provider things here in the future (e.g. supported categories, models, countries, etc.)...
    }
}
