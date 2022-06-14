namespace HardwareServiceOrder.API.ViewModels
{
    public class CustomerServiceProvider
    {
        /// <summary>
        /// Provider identifier
        /// </summary>
        public int ProviderId { get; set; }

        /// <summary>
        /// List of asset categories supported by the provider
        /// </summary>
        public List<int> AssetCategoryIds { get; set; }

        /// <summary>
        /// Username for calling service provider's API
        /// </summary>
        public string? ApiUserName { get; set; }

        /// <summary>
        /// Password for call service provider's API
        /// </summary>
        public string? ApiPassword { get; set; }
    }
}
