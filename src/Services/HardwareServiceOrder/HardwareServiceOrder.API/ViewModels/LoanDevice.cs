namespace HardwareServiceOrder.API.ViewModels
{
    public class LoanDevice
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid CallerId { get; set; }
        public LoanDevice(string phone, string email)
        {
            PhoneNumber = phone;
            Email = email;
        }
        public LoanDevice()
        {

        }
    }
}
