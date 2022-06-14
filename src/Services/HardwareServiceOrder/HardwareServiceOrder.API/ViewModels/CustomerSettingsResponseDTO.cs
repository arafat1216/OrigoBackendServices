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

        public string? ServiceId { get; set; }
    }
}
