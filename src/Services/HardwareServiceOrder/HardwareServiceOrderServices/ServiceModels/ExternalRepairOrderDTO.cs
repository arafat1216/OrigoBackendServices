namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    ///     Represents the repair-order details that is retrieved from the external service-provider.
    /// </summary>
    public class ExternalRepairOrderDTO : ExternalServiceOrderDTO
    {
        /// <summary>
        ///     Restricted constructor reserved for JSON serializers.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ExternalRepairOrderDTO() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            
        }


        public ExternalRepairOrderDTO(string serviceProviderOrderId1,
                                      string? serviceProviderOrderId2,
                                      IEnumerable<ExternalServiceEventDTO>? externalServiceEvents,
                                      AssetInfoDTO? providedAsset)
                                     : base(serviceProviderOrderId1, serviceProviderOrderId2, externalServiceEvents, providedAsset)
        {

        }

        public ExternalRepairOrderDTO(string serviceProviderOrderId1,
                                      string? serviceProviderOrderId2,
                                      IEnumerable<ExternalServiceEventDTO>? externalServiceEvents,
                                      AssetInfoDTO? providedAsset,
                                      AssetInfoDTO? returnedAsset,
                                      bool? assetIsReplaced)
                                      : base(serviceProviderOrderId1, serviceProviderOrderId2, externalServiceEvents, providedAsset)
        {
            ReturnedAsset = returnedAsset;
            AssetIsReplaced = assetIsReplaced;
        }


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

    }
}
