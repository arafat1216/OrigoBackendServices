namespace HardwareServiceOrderServices.ServiceModels
{
    public class CustomerSettingsDTO
    {
        public string ServiceId { get; set; }
        public string LoanDevicePhoneNumber { get; set; }
        public string LoanDeviceEmail { get; set; }
        public Guid CustomerId { get; set; }
    }
}
