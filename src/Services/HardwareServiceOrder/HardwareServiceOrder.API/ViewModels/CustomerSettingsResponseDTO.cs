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

        [JsonPropertyName("serviceId")]
        public string? ApiUsername { get; set; }
    }
}
