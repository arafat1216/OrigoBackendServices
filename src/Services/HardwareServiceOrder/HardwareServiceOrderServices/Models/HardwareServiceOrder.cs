using Common.Seedwork;
using System.ComponentModel.DataAnnotations.Schema;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///    Represents a single hardware service-order.
    /// </summary>
    public class HardwareServiceOrder : EntityV2, IAggregateRoot
    {
        /// <summary>
        ///     Constructor reserved for Entity Framework and testing.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public HardwareServiceOrder() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }


        /// <summary>
        ///     Used for creating a new service-order after it's been registered in the service-provider's system.
        /// </summary>
        /// <param name="callerId"> The user that placed the service-request. </param>
        /// <param name="customerId"> The ID for the customer's organization. </param>
        /// <param name="assetLifecycleId"> The items 'asset life-cycle ID'. </param>
        /// <param name="userDescription"> The users description. This is typically a fault-description stating whats wrong, and what needs to be done. </param>
        /// <param name="owner"> The user that owns and handles the service-order. </param>
        /// <param name="deliveryAddress"> The address that is used for all shipping/return labels to and from the customer/user. </param>
        /// <param name="serviceTypeId"> The <see cref="ServiceType.Id">service-type</see> that is used for this service. </param>
        /// <param name="statusId"> The current <see cref="ServiceStatus.Id">service-status</see> for this service. </param>
        /// <param name="serviceProviderId"> The ID for the <see cref="Models.ServiceProvider">service-provider</see> that is handling this service. </param>
        /// <param name="serviceProviderOrderId1">  </param>
        /// <param name="serviceProviderOrderId2">  </param>
        /// <param name="externalServiceManagementLink">  </param>
        /// <param name="serviceEvents"> The service-events that was received from the service-provider. </param>
        public HardwareServiceOrder(Guid callerId, Guid customerId, Guid assetLifecycleId, string userDescription, ContactDetails owner, DeliveryAddress? deliveryAddress, int serviceTypeId, int statusId, int serviceProviderId, string serviceProviderOrderId1, string? serviceProviderOrderId2, string? externalServiceManagementLink, IEnumerable<ServiceEvent> serviceEvents) : base()
        {
            //base.CreatedBy = callerId;

            CustomerId = customerId;
            AssetLifecycleId = assetLifecycleId;
            UserDescription = userDescription;
            ExternalServiceManagementLink = externalServiceManagementLink;
            ServiceProviderOrderId1 = serviceProviderOrderId1;
            ServiceProviderOrderId2 = serviceProviderOrderId2;
            Owner = owner;
            DeliveryAddress = deliveryAddress;
            ServiceEvents = serviceEvents;
            ServiceTypeId = serviceTypeId;
            StatusId = statusId;
            ServiceProviderId = serviceProviderId;
        }


        /// <inheritdoc cref="HardwareServiceOrder.HardwareServiceOrder(Guid, Guid, Guid, string, ContactDetails, DeliveryAddress?, int, int, int, string, string?, string?, IEnumerable{ServiceEvent})"/>
        /// <param name="externalId"> The external ID that uniquely identifies this service-request. </param>
        public HardwareServiceOrder(
            Guid callerId, 
            Guid externalId, 
            Guid customerId, 
            Guid assetLifecycleId, 
            string userDescription, 
            ContactDetails owner, 
            DeliveryAddress? deliveryAddress, 
            int serviceTypeId, 
            int statusId, 
            int serviceProviderId, 
            string serviceProviderOrderId1, 
            string? serviceProviderOrderId2, 
            string? externalServiceManagementLink, 
            IEnumerable<ServiceEvent> serviceEvents) : base()
        {
            //base.CreatedBy = callerId;

            ExternalId = externalId;
            CustomerId = customerId;
            AssetLifecycleId = assetLifecycleId;
            UserDescription = userDescription;
            ExternalServiceManagementLink = externalServiceManagementLink;
            ServiceProviderOrderId1 = serviceProviderOrderId1;
            ServiceProviderOrderId2 = serviceProviderOrderId2;
            Owner = owner;
            DeliveryAddress = deliveryAddress;
            ServiceEvents = serviceEvents;
            ServiceTypeId = serviceTypeId;
            StatusId = statusId;
            ServiceProviderId = serviceProviderId;
        }



        [Obsolete()]
        public HardwareServiceOrder(
            Guid customerId,
            ContactDetails owner,
            Guid assetLifecycleId,
            DeliveryAddress deliveryAddress,
            string userDescription,
            ServiceProvider serviceProvider,
            string serviceProviderOrderId1,
            string? serviceProviderOrderId2,
            string? externalServiceManagementLink,
            ServiceType serviceType,
            ServiceStatus status)
        {
            CustomerId = customerId;
            Owner = owner;
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
        /// <param name="owner"></param>
        /// <param name="assetLifecycleId"></param>
        /// <param name="deliveryAddress"></param>
        /// <param name="userDescription"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="serviceProviderOrderId1"></param>
        /// <param name="serviceProviderOrderId2"></param>
        /// <param name="externalServiceManagementLink"></param>
        /// <param name="serviceType"></param>
        /// <param name="status"></param>
        /// <param name="createdDate"></param>
        [Obsolete()]
        public HardwareServiceOrder(
            Guid customerId,
            ContactDetails owner,
            Guid assetLifecycleId,
            DeliveryAddress deliveryAddress,
            string userDescription,
            ServiceProvider serviceProvider,
            string serviceProviderOrderId1,
            string? serviceProviderOrderId2,
            string? externalServiceManagementLink,
            ServiceType serviceType,
            ServiceStatus status,
            DateTimeOffset createdDate
        ) : base(Guid.Empty, createdDate, null, null)
        {
            CustomerId = customerId;
            Owner = owner;
            AssetLifecycleId = assetLifecycleId;
            DeliveryAddress = deliveryAddress;
            UserDescription = userDescription;
            ServiceProvider = serviceProvider;
            ExternalServiceManagementLink = externalServiceManagementLink;
            ServiceProviderOrderId1 = serviceProviderOrderId1;
            ServiceProviderOrderId2 = serviceProviderOrderId2;
            ServiceType = serviceType;
            Status = status;
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
        ///     The identifier for the asset life-cycle attached to the service order.
        /// </summary>
        public Guid AssetLifecycleId { get; set; }

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

        /// <summary>
        /// To ensure whether an email is already sent for returning loan device
        /// </summary>
        public bool IsReturnLoanDeviceEmailSent { get; set; }

        /// <summary>
        /// To ensure whether an email is already sent for discarding order
        /// </summary>
        public bool IsOrderDiscardedEmailSent { get; set; }

        /// <summary>
        /// To ensure whether an email is already sent for canceled order
        /// </summary>
        public bool IsOrderCancellationEmailSent { get; set; }


        /*
         * EF owned properties.
         * 
         * Note: These are always loaded/included.
         */

        /// <summary>
        ///     The user who owns/handles the service order.
        /// </summary>
        public virtual ContactDetails Owner { get; set; }

        /// <summary>
        ///     The delivery address used when returning the completed service order. <para>
        ///     
        ///     This is required for repairs, but will be <see langword="null"/> for aftermarket services.</para>
        /// </summary>
        public DeliveryAddress? DeliveryAddress { get; set; }

        /// <summary>
        ///     Backing field for <see cref="ServiceEvents"/>
        /// </summary>
        [NotMapped]
        private List<ServiceEvent> _serviceEvents = null!;

        /// <summary>
        ///     The recorded external service-events for this order.
        /// </summary>
        public virtual IEnumerable<ServiceEvent> ServiceEvents
        {
            get { return _serviceEvents; }
            init { _serviceEvents = value.ToList(); }
        }


        /*
         * EF Navigation properties w/fields for exposing their foreign-key IDs.
         * 
         * Note: The navigation properties must be explicitly include in the EF query.
         */

        /// <summary>
        ///     The foreign-key value recorded for the <see cref="ServiceType"/> navigation property.
        /// </summary>
        public int ServiceTypeId { get; set; }

        /// <summary>
        ///     The type of service that has been requested.
        /// </summary>
        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType? ServiceType { get; set; }


        /// <summary>
        ///     The foreign-key value recorded for the <see cref="Status"/> navigation property.
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        ///     The current status for the service-order.
        /// </summary>
        [ForeignKey("StatusId")]
        public virtual ServiceStatus? Status { get; set; }


        /// <summary>
        ///     The service-provider that is handling the service.
        /// </summary>
        [ForeignKey("ServiceProviderId")]
        public virtual ServiceProvider? ServiceProvider { get; set; }

        /// <summary>
        ///     The foreign-key value recorded for the <see cref="ServiceProvider"/> navigation property.
        /// </summary>
        public int ServiceProviderId { get; init; }


        /*
         * Methods
         */

        /// <summary>
        ///     Adds a new item to <see cref="ServiceEvents"/>.
        /// </summary>
        /// <param name="serviceEvent"> The service-event to be added. </param>
        public void AddServiceEvent(ServiceEvent serviceEvent)
        {
            _serviceEvents.Add(serviceEvent);
        }


        /// <summary>
        ///     Retrieves the newest timestamp that exist in the recorded service-events.
        /// </summary>
        /// <returns> Returns <see langword="null"/> if no events are present, otherwise it returns 
        ///     the <see cref="ServiceEvent.Timestamp">timestamp</see> for the newest event. </returns>
        public DateTimeOffset? NewestEventTimestamp()
        {
            return ServiceEvents.OrderByDescending(e => e.Timestamp)
                                .Select(e => e.Timestamp)
                                .FirstOrDefault();
        }
    }
}
