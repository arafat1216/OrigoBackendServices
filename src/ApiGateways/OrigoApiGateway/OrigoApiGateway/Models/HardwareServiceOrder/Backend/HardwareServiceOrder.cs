using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response;

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend
{
    public class HardwareServiceOrder
    {
        /// <summary>
        ///     The service-order ID.
        /// </summary>
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public Guid Id { get; set; }

        // This is a quickfix, as we are returning the DTO, but we want to remap (rename) the DTOs "ExternalID" property to "ID" in this model.
        [Obsolete("A temp. backend-only quickfix. Should not be used by frontend as it will soon be removed!")]
        public Guid ExternalId
        {
            set
            {
                if (Id == Guid.Empty)
                    Id = value;
            }
        }

        /// <summary>
        ///     The ID of the customer the service-order is attached to.
        /// </summary>
        [Required]
        public Guid CustomerId { get; set; }

        /// <summary>
        ///     The ID of the asset life-cycle the service-order is attached to.
        /// </summary>
        [Required]
        public Guid AssetLifecycleId { get; set; }

        /// <summary>
        ///     The asset-category that is associated with the <c><see cref="AssetLifecycleId"/></c>.
        /// </summary>
        [Required]
        public int AssetLifecycleCategoryId { get; set; }

        /// <summary>
        ///     The asset details that was provided when the service-order was created.
        /// </summary>
        [Required]
        public AssetInfo AssetInfo { get; set; }

        /// <summary>
        ///     The asset information that is returned from the service-provider once the service has been completed.
        ///     
        ///     <br/><br/>
        ///     In most cases this object, or most of it's properties will be <c><see langword="null"/></c>. Typically only new
        ///     or changed values are recorded here, such as new IMEI or serial-number when the user receive a replacement device.
        /// </summary>
        [SwaggerSchema(ReadOnly = true)]
        public AssetInfo? ReturnedAssetInfo { get; set; }

        /// <summary>
        ///     A user provided description explaining the problem or reason for the service order.
        /// </summary>
        [Required]
        public string UserDescription { get; set; }

        /// <summary>
        ///     The user who owns/handles the service order.
        /// </summary>
        [Required]
        public ContactDetails Owner { get; set; }

        /// <summary>
        ///     The delivery address used when returning the completed service order.
        ///     
        ///     <para>
        ///     This is also used as the "return to sender" address in shipping-labels. </para>
        /// </summary>
        [Required]
        public DeliveryAddress? DeliveryAddress { get; set; }

        /// <summary>
        ///     The identifier for the service-type that has been registered on this service-order.
        /// </summary>
        [Required]
        public int ServiceTypeId { get; set; }

        /// <summary>
        ///     The identifier for the service-order's current status.
        /// </summary>
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public int StatusId { get; set; }

        /// <summary>
        ///     The identifier for service-provider that handles this service-order.
        /// </summary>
        [Required]
        public int ServiceProviderId { get; set; }

        /// <summary>
        ///     A list containing the <see cref="ServiceOrderAddon"/> IDs that was included with the order.
        /// </summary>
        [SwaggerSchema(ReadOnly = true)]
        public IReadOnlyCollection<int>? IncludedServiceOrderAddonIds { get; init; }

        /// <summary>
        ///     A URL to the service-provider's service-status/management system. This is usually a portal where the user can 
        ///     interact with, and/or see information about the service.
        /// </summary>
        [SwaggerSchema(ReadOnly = true)]
        public string? ExternalServiceManagementLink { get; set; }

        /// <summary>
        ///      The recorded external service-events for this order.
        /// </summary>
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public IEnumerable<ServiceEvent> ServiceEvents { get; set; }

        /// <summary>
        ///     A timestamp for when the order was initially created.
        /// </summary>
        public DateTimeOffset? DateCreated { get; init; }

        /// <summary>
        ///     A timestamp for when the order was last updated.
        /// </summary>
        public DateTimeOffset? DateUpdated { get; init; }
    }
}