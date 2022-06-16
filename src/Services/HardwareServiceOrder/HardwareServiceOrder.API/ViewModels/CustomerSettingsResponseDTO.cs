using System.Text.Json.Serialization;

namespace HardwareServiceOrder.API.ViewModels
{
    public class CustomerSettingsResponseDTO : CustomerSettings
    {
        public CustomerSettingsResponseDTO()
        {

        }

        public CustomerSettingsResponseDTO(CustomerSettings customerSettings)
        {
            CustomerId = customerSettings.CustomerId;
            LoanDevice = customerSettings.LoanDevice;
        }
        /// TODO: Now it's required to support frontend. [JsonPropertyName("serviceId")] should be removed in later PR
        [JsonPropertyName("serviceId")]
        public string? ApiUsername { get; set; }
    }
}
