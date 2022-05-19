namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    ///     Represents the repair-order details that is retrieved from the external service-provider.
    /// </summary>
    public class ExternalRepairOrderDTO
    {
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


        public ExternalRepairOrderDTO()
        {
            ExternalServiceEvents = new List<ExternalServiceEventDTO>();
        }


        public ExternalRepairOrderDTO(IEnumerable<ExternalServiceEventDTO> externalEvents, AssetInfoDTO? providedAsset)
        {
            ExternalServiceEvents = externalEvents;
            ProvidedAsset = providedAsset;
        }

        public ExternalRepairOrderDTO(IEnumerable<ExternalServiceEventDTO> externalEvents, AssetInfoDTO? providedAsset, AssetInfoDTO? returnedAsset, bool? assetIsReplaced)
        {
            ExternalServiceEvents = externalEvents;
            ProvidedAsset = providedAsset;
            ReturnedAsset = returnedAsset;
            AssetIsReplaced = assetIsReplaced;
        }

    }
}
