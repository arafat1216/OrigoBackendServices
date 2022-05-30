namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    ///     Represents the repair-order details that is retrieved from the external service-provider.
    /// </summary>
    public class ExternalRepairOrderDTO
    {
        /// <inheritdoc cref="Models.HardwareServiceOrder.ServiceProviderOrderId1"/>
        public string ServiceProviderOrderId1 { get; set; }

        /// <inheritdoc cref="Models.HardwareServiceOrder.ServiceProviderOrderId2"/>
        public string? ServiceProviderOrderId2 { get; set; }

        /// <summary>
        ///     A collection of all relevant events and status-updates that has been retrieved from the service-provider.
        /// </summary>
        public IEnumerable<ExternalServiceEventDTO> ExternalServiceEvents { get; set; }

        /// <summary>
        ///     If supported by the service-provider, details about the asset that was handed in.
        /// </summary>
        public AssetInfoDTO? ProvidedAsset { get; set; }

        /// <summary>
        ///     If available, information about the returned asset.
        /// </summary>
        public AssetInfoDTO? ReturnedAsset { get; set; }

        /// <summary>
        ///     This is <see langword="null"/> when a service is ongoing, or if asset-replacement is not supported/applicable for the current 
        ///     service-request. Once a service has been completed, the value will be <see langword="true"/> if the asset was replaced/swapped, 
        ///     or <see langword="false"/> if the service-provider indicated that no replacement has taken place.
        /// </summary>
        public bool? AssetIsReplaced { get; set; }

        /// <summary>
        ///     Restricted constructor reserved for JSON serializers.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ExternalRepairOrderDTO()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            ExternalServiceEvents = new List<ExternalServiceEventDTO>();
        }


        public ExternalRepairOrderDTO(string serviceProviderOrderId1, string? serviceProviderOrderId2, IEnumerable<ExternalServiceEventDTO> externalEvents, AssetInfoDTO? providedAsset)
        {
            ServiceProviderOrderId1 = serviceProviderOrderId1;
            ServiceProviderOrderId2 = serviceProviderOrderId2;
            ExternalServiceEvents = externalEvents;
            ProvidedAsset = providedAsset;
        }

        public ExternalRepairOrderDTO(string serviceProviderOrderId1, string? serviceProviderOrderId2, IEnumerable<ExternalServiceEventDTO> externalEvents, AssetInfoDTO? providedAsset, AssetInfoDTO? returnedAsset, bool? assetIsReplaced)
        {
            ServiceProviderOrderId1 = serviceProviderOrderId1;
            ServiceProviderOrderId2 = serviceProviderOrderId2;
            ExternalServiceEvents = externalEvents;
            ProvidedAsset = providedAsset;
            ReturnedAsset = returnedAsset;
            AssetIsReplaced = assetIsReplaced;
        }

    }
}
