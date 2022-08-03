using Common.Seedwork;
using System.ComponentModel.DataAnnotations.Schema;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    /// Mapping between customer and service provider
    /// </summary>
    public class CustomerServiceProvider : EntityV2
    {
        /// <summary>
        ///     Customer identifier
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        ///     Service provider identifier
        /// </summary>
        public int ServiceProviderId { get; set; }

        /// <summary>
        ///     Username for accessing providers APIs
        /// </summary>
        public string? ApiUserName { get; set; }

        /// <summary>
        ///     Password for accessing providers APIs
        /// </summary>
        public string? ApiPassword { get; set; }

        /// <summary>
        ///     Retrieve updates that has been made after this timestamp. 
        /// </summary>
        public DateTimeOffset LastUpdateFetched { get; set; }


        /*
         * EF navigation properties
         */

        /// <summary>
        ///     Navigation property for <see cref="Models.ServiceProvider"/>
        /// </summary>
        [ForeignKey(nameof(ServiceProviderId))]
        public virtual ServiceProvider? ServiceProvider { get; set; }
    }
}
