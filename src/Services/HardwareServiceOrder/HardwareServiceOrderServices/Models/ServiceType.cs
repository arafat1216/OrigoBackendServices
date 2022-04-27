using Common.Seedwork;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents the type of service. E.g. 'SUR', 'recycle', etc.
    /// </summary>
    public class ServiceType : Entity
    {
        /// <summary>
        ///     The entities internal database ID.
        /// </summary>
        public new ServiceTypeEnum Id { get; set; }
    }
}