namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    ///     Contains the new/updated data that is returned by the external service-provider after a new repair order has been created.
    ///     What information that's available may differ between providers.
    /// </summary>
    public class NewExternalRepairOrderResponseDTO
    {

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
        ///     A URL to the service-provider's service-status/management system. This is usually a portal where the user can 
        ///     interact with, and/or see information about the service.
        /// </summary>
        public string? ExternalServiceManagementLink { get; set; }


        /// <summary>
        ///     Initializes a new instance of the <see cref="NewExternalRepairOrderResponseDTO"/> class.
        /// </summary>
        /// <param name="serviceProviderOrderId1"> The identifier that was provided by the service-provider. This is what's used when looking up the 
        ///     service-order in the provider's systems/APIs. </param>
        /// <param name="serviceProviderOrderId2"> The identifier that was provided by the service-provider. This is what's used when looking up the
        ///     service-order in the provider's systems/APIs. </param>
        /// <param name="externalServiceManagementLink"> A URL to the service-provider's service-status/management system. This is usually a portal 
        ///     where the user can interact with, and/or see information about the service. </param>
        public NewExternalRepairOrderResponseDTO(string serviceProviderOrderId1, string? serviceProviderOrderId2 = null, string? externalServiceManagementLink = null)
        {
            ServiceProviderOrderId1 = serviceProviderOrderId1;
            ServiceProviderOrderId2 = serviceProviderOrderId2;
            ExternalServiceManagementLink = externalServiceManagementLink;
        }
    }
}
