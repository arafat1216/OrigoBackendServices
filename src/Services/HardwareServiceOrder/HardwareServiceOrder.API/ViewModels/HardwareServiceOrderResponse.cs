using Common.Seedwork;

namespace HardwareServiceOrder.API.ViewModels
{
    public class HardwareServiceOrderResponse
    {
        /// <inheritdoc cref="HardwareServiceOrderDTO.ExternalId"/>
        /// <example> 00000000-0000-0000-0000-000000000000 </example>
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public Guid Id { get; set; }

        /// <inheritdoc cref="HardwareServiceOrderDTO.CustomerId"/>
        /// <example> 00000000-0000-0000-0000-000000000000 </example>
        [Required]
        public Guid CustomerId { get; set; }

        /// <inheritdoc cref="HardwareServiceOrderDTO.AssetLifecycleId"/>
        /// <example> 00000000-0000-0000-0000-000000000000 </example>
        [Required]
        public Guid AssetLifecycleId { get; set; }

        /// <inheritdoc cref="HardwareServiceOrderDTO.AssetInfo"/>
        [Required]
        public AssetInfoDTO AssetInfo { get; set; }

        /// <inheritdoc cref="HardwareServiceOrderDTO.ReturnedAssetInfo"/>
        [SwaggerSchema(ReadOnly = true)]
        public AssetInfoDTO? ReturnedAssetInfo { get; set; }

        /// <inheritdoc cref="HardwareServiceOrderDTO.UserDescription"/>
        /// <example> I dropped the device, and now the screen needs to be replaced. </example>
        [Required]
        public string UserDescription { get; set; }

        /// <inheritdoc cref="HardwareServiceOrderDTO.Owner"/>
        [Required]
        public ContactDetailsDTO Owner { get; set; }

        /// <inheritdoc cref="HardwareServiceOrderDTO.DeliveryAddress"/>
        [Required]
        public DeliveryAddressDTO? DeliveryAddress { get; set; }

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
        public int ServiceProviderId { get; init; }

        /// <inheritdoc cref="HardwareServiceOrderDTO.ExternalServiceManagementLink"/>
        /// <example> https://www.my-service-provider.com/order/123456?username=1234&password=1234 </example>
        [SwaggerSchema(ReadOnly = true)]
        public string? ExternalServiceManagementLink { get; set; }

        /// <inheritdoc cref="HardwareServiceOrderDTO.ServiceEvents"/>
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public IEnumerable<ExternalServiceEventDTO> ServiceEvents { get; set; }

        /// <inheritdoc cref="Auditable.DateCreated"/>
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public DateTimeOffset DateCreated { get; init; }

        /// <inheritdoc cref="Auditable.CreatedBy"/>
        [SwaggerSchema(ReadOnly = true)]
        public Guid CreatedBy { get; init; }

        /// <inheritdoc cref="Auditable.DateUpdated"/>
        [SwaggerSchema(ReadOnly = true)]
        public DateTimeOffset? DateUpdated { get; init; }

        /// <inheritdoc cref="Auditable.UpdatedBy"/>
        [SwaggerSchema(ReadOnly = true)]
        public Guid? UpdatedBy { get; init; }
    }
}
