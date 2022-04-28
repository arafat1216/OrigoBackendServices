using Common.Seedwork;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents the type of service. E.g. 'SUR', 'recycle', etc.
    /// </summary>
    public class ServiceType : EntityV2
    {
        /// <inheritdoc cref="Entity.Id"/>
        /// <remarks>
        ///     The value-mappings can be retrieved from <see cref="ServiceTypeEnum"/>.
        /// </remarks>
        public new int Id { get; set; }
    }
}