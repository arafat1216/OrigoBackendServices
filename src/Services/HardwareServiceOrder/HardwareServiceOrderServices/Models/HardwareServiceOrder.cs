using Common.Seedwork;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Entity Framework model that represents a single hardware service order.
    /// </summary>
    public class HardwareServiceOrder : Entity, IAggregateRoot
    {
        /// <summary>
        ///     Constructor reserved for Entity Framework
        /// </summary>
        private HardwareServiceOrder() { }

        // TODO: Add service provider parameter
        public HardwareServiceOrder(Guid assetLifecycleId, DeliveryAddress deliveryAddress, string userDescription, string? externalProviderLink, ServiceType serviceType, Status status)
        {
            AssetLifecycleId = assetLifecycleId;
            DeliveryAddress = deliveryAddress;
            UserDescription = userDescription;
            ExternalProviderLink = externalProviderLink;
            ServiceType = serviceType;
            Status = status;
        }

        /// <summary>
        ///     The service-ID that is used as an identifier outside the microservice.
        /// </summary>
        public Guid ExternalId { get; set; }

        /// <summary>
        ///     The identifier for the asset lifecycle attached to the service order.
        /// </summary>
        public Guid AssetLifecycleId { get; set; }

        /// <summary>
        ///     The delivery address used when returning the completed service order. <para>
        ///     
        ///     This is required for repairs, but will be <see langword="null"/> for aftermarket services.</para>
        /// </summary>
        public DeliveryAddress? DeliveryAddress { get; set; }

        /// <summary>
        ///     A user provided description explaining the problem or reason for the service order.
        /// </summary>
        public string UserDescription { get; set; }

        /// <summary>
        ///     A URL to the service-providers status-system. Usually this offers a portal where the user can 
        ///     interact with, and/or see information about the service.
        /// </summary>
        public string? ExternalProviderLink { get; set; }


        /*
         * Property IDs
         */

        /// <summary>
        ///     The foreign key value recorded for the <see cref="ServiceType"/> navigation property.
        /// </summary>
        public int ServiceTypeId { get; set; }

        /// <summary>
        ///     The foreign key value recorded for the <see cref="Status"/> navigation property.
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        ///     The foreign key value recorded for the <see cref="ServiceProvider"/> navigation property.
        /// </summary>
        //public int ServiceProviderId { get; init; }


        /*
         * EF Navigation properties
         */

        /// <summary>
        ///     The type of service that has been requested.
        /// </summary>
        public virtual ServiceType ServiceType { get; set; }

        /// <summary>
        ///     The current status for the service-order.
        /// </summary>
        public virtual Status Status { get; set; }

        //public virtual ServiceProvider {get; set;}
    }
}
