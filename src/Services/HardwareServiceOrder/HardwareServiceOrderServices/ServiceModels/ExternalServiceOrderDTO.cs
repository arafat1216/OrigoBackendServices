namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    ///     Represents the shared/common service-order details that is retrieved from the external service-provider.
    /// </summary>
    public abstract class ExternalServiceOrderDTO
    {
        /// <summary>
        ///     Restricted constructor reserved for JSON serializers.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ExternalServiceOrderDTO()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            ExternalServiceEvents ??= new List<ExternalServiceEventDTO>();
        }

        protected ExternalServiceOrderDTO(string serviceProviderOrderId1,
                                          string? serviceProviderOrderId2,
                                          IEnumerable<ExternalServiceEventDTO>? externalServiceEvents,
                                          AssetInfoDTO? providedAsset)
        {
            ServiceProviderOrderId1 = serviceProviderOrderId1;
            ServiceProviderOrderId2 = serviceProviderOrderId2;
            ExternalServiceEvents = externalServiceEvents ?? new List<ExternalServiceEventDTO>();
            ProvidedAsset = providedAsset;
        }


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
    }
}
