namespace OrigoApiGateway.Services
{
    public class HardwareServiceOrderConfiguration : IBaseGatewayOptions
    {
        /// <summary>
        ///     The API base-path for the legacy 'hardware-repair' controller.
        /// </summary>
        [Obsolete("This is the API base-path for the legacy hardware-repair controller.")]
        public string ApiPath { get; set; }

        /// <summary>
        ///     The API base-path to the 'service-provider' controller.
        /// </summary>
        public string ServiceProviderApiPath { get; set; }

        /// <summary>
        ///     The API base-path to the 'configuration' controller.
        /// </summary>
        public string ConfigurationApiPath { get; set; }
    }
}
