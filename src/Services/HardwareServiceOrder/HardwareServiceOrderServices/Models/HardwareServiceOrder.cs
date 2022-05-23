using Common.Seedwork;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///    Represents a single hardware service-order.
    /// </summary>
    public class HardwareServiceOrder : EntityV2, IAggregateRoot
    {
        /// <summary>
        ///     Constructor reserved for Entity Framework
        /// </summary>
        private HardwareServiceOrder() { }

        // TODO: Add service provider parameter


        public HardwareServiceOrder(Guid customerId, User orderedBy, Guid assetLifecycleId, DeliveryAddress deliveryAddress, string userDescription, ServiceProvider serviceProvider, string serviceProviderOrderId1, string? serviceProviderOrderId2, string? externalServiceManagementLink, ServiceType serviceType, ServiceStatus status)
        {
            CustomerId = customerId;
            OrderedBy = orderedBy;
            AssetLifecycleId = assetLifecycleId;
            DeliveryAddress = deliveryAddress;
            UserDescription = userDescription;
            ServiceProvider = serviceProvider;
            ServiceProviderOrderId1 = serviceProviderOrderId1;
            ServiceProviderOrderId2 = serviceProviderOrderId2;
            ExternalServiceManagementLink = externalServiceManagementLink;
            ServiceType = serviceType;
            Status = status;
        }

        /// <summary>
        ///     Constructor for unit testing.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="orderedBy"></param>
        /// <param name="assetLifecycleId"></param>
        /// <param name="assetName"></param>
        /// <param name="deliveryAddress"></param>
        /// <param name="userDescription"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="serviceProviderOrderId1"></param>
        /// <param name="serviceProviderOrderId2"></param>
        /// <param name="externalServiceManagementLink"></param>
        /// <param name="serviceType"></param>
        /// <param name="status"></param>
        /// <param name="createdDate"></param>
        public HardwareServiceOrder(
            Guid customerId, 
            User orderedBy,
            Guid assetLifecycleId,
            string assetName,
            DeliveryAddress deliveryAddress,
            string userDescription,
            ServiceProvider serviceProvider,
            string serviceProviderOrderId1,
            string? serviceProviderOrderId2,
            string? externalServiceManagementLink,
            ServiceType serviceType,
            ServiceStatus status,
            DateTimeOffset createdDate)
        {
            CustomerId = customerId;
            OrderedBy = orderedBy;
            AssetLifecycleId = assetLifecycleId;
            AssetName = assetName;
            DeliveryAddress = deliveryAddress;
            UserDescription = userDescription;
            ServiceProvider = serviceProvider;
            ExternalServiceManagementLink = externalServiceManagementLink;
            ServiceProviderOrderId1 = serviceProviderOrderId1;
            ServiceProviderOrderId2 = serviceProviderOrderId2;
            ServiceType = serviceType;
            Status = status;
            CreatedDate = createdDate;
        }

        /// <summary>
        ///     The service-ID that is used as an identifier outside the microservice.
        /// </summary>
        public Guid ExternalId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// ID of the customer
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        ///     The identifier for the asset lifecycle attached to the service order.
        /// </summary>
        public Guid AssetLifecycleId { get; set; }

        /// <summary>
        ///     The name of the asset
        /// </summary>
        public string AssetName { get; set; }

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
        ///     A URL to the service-provider's service-status/management system. This is usually a portal where the user can 
        ///     interact with, and/or see information about the service.
        /// </summary>
        public string? ExternalServiceManagementLink { get; set; }

        /// <summary>
        ///     The identifier that was provided by the service-provider. This is what's used when looking up the service-order
        ///     in the provider's systems/APIs.
        /// </summary>
        public string ServiceProviderOrderId1 { get; set; }

        /// <summary>
        ///     The identifier that was provided by the service-provider. This is what's used when looking up the service-order
        ///     in the provider's systems/APIs. If the service-provider don't use several identifiers, then this will be <see langword="null"/>.
        /// </summary>
        public string? ServiceProviderOrderId2 { get; set; }


        /*
         * Property IDs
         */

        /// <summary>
        ///     The foreign key value recorded for the <see cref="ServiceType"/> navigation property.
        /// </summary>
        //public int ServiceTypeId { get; set; }

        /// <summary>
        ///     The foreign key value recorded for the <see cref="Status"/> navigation property.
        /// </summary>
        //public int StatusId { get; set; }

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
        public virtual ServiceStatus Status { get; set; }

        /// <summary>
        ///     The service-provider that is handling the service.
        /// </summary>
        public virtual ServiceProvider ServiceProvider { get; set; }

        /// <summary>
        ///     The user who placed the order
        /// </summary>
        public virtual User OrderedBy { get; set; }

        /// <summary>
        /// To ensure whether an email is already sent for returning loan device
        /// </summary>
        public bool IsReturnLoanDeviceEmailSent { get; set; }

        /// <summary>
        /// To ensure whether an email is already sent for discardig order
        /// </summary>
        public bool IsOrderDiscardedEmailSent { get; set; }

        /// <summary>
        /// To ensure whether an email is already sent for cancelled order
        /// </summary>
        public bool IsOrderCancellationEmailSent { get; set; }
    }
}
