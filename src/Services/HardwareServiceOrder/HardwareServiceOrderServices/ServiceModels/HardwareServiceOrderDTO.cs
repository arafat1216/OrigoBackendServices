using Common.Seedwork;
using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.ServiceModels
{
    public class HardwareServiceOrderDTO
    {
        /// <inheritdoc cref="HardwareServiceOrder.ExternalId"/>
        /// <remarks>
        ///     For existing entities, this property is read-only, and can never be <see langword="null"/>.
        /// </remarks>
        public Guid? ExternalId { get; init; }

        /// <inheritdoc cref="HardwareServiceOrder.CustomerId"/>
        public Guid CustomerId { get; set; }

        /// <inheritdoc cref="HardwareServiceOrder.AssetLifecycleId"/>
        public Guid AssetLifecycleId { get; set; }


        public int AssetLifecycleCategoryId { get; set; }

        /// <inheritdoc cref="HardwareServiceOrder.AssetInfo"/>
        public AssetInfoDTO AssetInfo { get; set; }

        /// <inheritdoc cref="HardwareServiceOrder.ReturnedAssetInfo"/>
        public AssetInfoDTO? ReturnedAssetInfo { get; set; }

        /// <inheritdoc cref="HardwareServiceOrder.UserDescription"/>
        public string UserDescription { get; set; }

        /// <inheritdoc cref="HardwareServiceOrder.Owner"/>
        public ContactDetailsDTO Owner { get; set; }

        /// <inheritdoc cref="HardwareServiceOrder.DeliveryAddress"/>
        public DeliveryAddressDTO? DeliveryAddress { get; set; }

        /// <inheritdoc cref="HardwareServiceOrder.ServiceTypeId"/>
        public int ServiceTypeId { get; set; }

        /// <inheritdoc cref="HardwareServiceOrder.StatusId"/>
        public int StatusId { get; set; }

        /// <inheritdoc cref="HardwareServiceOrder.ServiceProviderId"/>
        public int ServiceProviderId { get; init; }

        /// <inheritdoc cref="HardwareServiceOrder.ServiceProviderOrderId1"/>
        public string ServiceProviderOrderId1 { get; set; }

        /// <inheritdoc cref="HardwareServiceOrder.ServiceProviderOrderId2"/>
        public string? ServiceProviderOrderId2 { get; set; }

        /// <inheritdoc cref="HardwareServiceOrder.ExternalServiceManagementLink"/>
        public string? ExternalServiceManagementLink { get; set; }

        /// <inheritdoc cref="HardwareServiceOrder.ServiceEvents"/>
        public IEnumerable<ExternalServiceEventDTO> ServiceEvents { get; set; }

        /// <inheritdoc cref="Auditable.DateCreated"/>
        /// <remarks>
        ///     This is a read-only property. This will never be <see langword="null"/> for existing entities.
        /// </remarks>
        public DateTimeOffset? DateCreated { get; init; }

        /// <inheritdoc cref="Auditable.CreatedBy"/>
        /// <remarks>
        ///     This is a read-only property. This will never be <see langword="null"/> for existing entities.
        /// </remarks>
        public Guid? CreatedBy { get; init; }

        /// <inheritdoc cref="Auditable.DateUpdated"/>
        /// <remarks>
        ///     This is a read-only property.
        /// </remarks>
        public DateTimeOffset? DateUpdated { get; init; }

        /// <inheritdoc cref="Auditable.UpdatedBy"/>
        /// <remarks>
        ///     This is a read-only property.
        /// </remarks>
        public Guid? UpdatedBy { get; init; }
    }
}
