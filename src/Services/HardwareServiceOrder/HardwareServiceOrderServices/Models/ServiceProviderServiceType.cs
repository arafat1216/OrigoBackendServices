using Common.Seedwork;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     This is the many-to-many join-entity between <see cref="Models.ServiceProvider"/> and <see cref="Models.ServiceType"/>,
    ///     and defines the service-types that can be used with a given service-provider.
    /// </summary>

    public class ServiceProviderServiceType : EntityV2
    {
        public ServiceProviderServiceType()
        {
        }

        public ServiceProviderServiceType(int serviceProviderId, int serviceTypeId)
        {
            ServiceProviderId = serviceProviderId;
            ServiceTypeId = serviceTypeId;
        }


        /*
         * Properties
         */

        public int ServiceProviderId { get; set; }

        public int ServiceTypeId { get; set; }


        /*
         * Navigation
         */

        [ForeignKey(nameof(ServiceProviderId))]
        public virtual ServiceProvider? ServiceProvider { get; set; }

        [ForeignKey(nameof(ServiceTypeId))]
        public ServiceType? ServiceType { get; set; }
    }
}
