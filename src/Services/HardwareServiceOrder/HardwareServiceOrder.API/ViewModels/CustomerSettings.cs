namespace HardwareServiceOrder.API.ViewModels
{
    public class CustomerSettings
    {
        public Guid? CustomerId { get; set; }
        public string ServiceId { get; set; }
        public LoanDevice? LoanDevice { get; set; }
    }
}
