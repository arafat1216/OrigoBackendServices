using Common.Seedwork;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents a single status. A status is a overall collection of <see cref="ServiceState">states</see>.
    /// </summary>
    /// <example> In Transit </example>
    public class ServiceStatus : Entity
    {
        // TODO: When we implement backend translations, this will be changed to use int as a datatype.
        public new ServiceStatusEnum Id { get; set; }

        // We will eventually map add a mapping to the state and the translations here
    }
}
