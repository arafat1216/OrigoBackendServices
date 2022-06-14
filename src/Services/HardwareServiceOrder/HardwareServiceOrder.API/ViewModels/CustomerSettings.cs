namespace HardwareServiceOrder.API.ViewModels
{
    public class CustomerSettings
    {
        public CustomerSettings()
        {

        }
        public Guid? CustomerId { get; set; }
        public LoanDevice? LoanDevice { get; set; }
    }
}
