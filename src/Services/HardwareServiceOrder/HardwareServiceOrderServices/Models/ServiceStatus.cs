using Common.Seedwork;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents a single status. A status is a overall collection of <see cref="ServiceState">states</see>.
    /// </summary>
    /// <example> In Transit </example>
    public class ServiceStatus : EntityV2
    {
        /// <inheritdoc cref="EntityV2.Id"/>
        /// <remarks>
        ///     The value-mappings can be retrieved from <see cref="ServiceStatusEnum"/>.
        /// </remarks>
        public new int Id { get; set; }

        // We will eventually map add a mapping to the state and the translations here
    }
}
