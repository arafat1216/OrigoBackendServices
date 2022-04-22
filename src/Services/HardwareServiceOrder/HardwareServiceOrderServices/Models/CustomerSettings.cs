using Common.Seedwork;

namespace HardwareServiceOrderServices.Models
{
    public class CustomerSettings : Entity, IAggregateRoot
    {
        public CustomerSettings(Guid customerId, string serviceId)
        {
            ServiceId = serviceId;
            CustomerId = customerId;
        }
        public string ServiceId { get; set; }
        public string LoanPhoneNumber { get; set; }
        public string LoanPhoneEmail { get; set; }
        public Guid CustomerId { get; set; }
    }
}
