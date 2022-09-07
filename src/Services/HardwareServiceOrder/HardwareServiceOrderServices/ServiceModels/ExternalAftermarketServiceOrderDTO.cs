namespace HardwareServiceOrderServices.ServiceModels
{
    internal class ExternalAftermarketServiceOrderDTO : ExternalServiceOrderDTO
    {
        /// <summary>
        ///     Restricted constructor reserved for JSON serializers.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ExternalAftermarketServiceOrderDTO() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }


        public ExternalAftermarketServiceOrderDTO(string serviceProviderOrderId1,
                                                  string? serviceProviderOrderId2,
                                                  IEnumerable<ExternalServiceEventDTO>? externalServiceEvents,
                                                  AssetInfoDTO? providedAsset)
                                                  : base(serviceProviderOrderId1, serviceProviderOrderId2, externalServiceEvents, providedAsset)
        {

        }


    }
}
