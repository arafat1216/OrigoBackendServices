using Common.Seedwork;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents the global service-settings for a single customer (organization).
    /// </summary>
    public class CustomerSettings : Entity, IAggregateRoot
    {
        public CustomerSettings(Guid customerId, string serviceId, Guid callerId)
        {
            ServiceId = serviceId;
            CustomerId = customerId;
            CreatedBy = callerId;
        }

        public CustomerSettings()
        {

        }

        public CustomerSettings(Guid customerId, string loanPhoneNumber, string loanPhoneEmail, Guid callerId)
        {
            CustomerId = customerId;
            LoanDeviceEmail = loanPhoneEmail;
            LoanDevicePhoneNumber = loanPhoneNumber;
            CreatedBy = callerId;
        }

        public string? ServiceId { get; set; }
        public string? LoanDevicePhoneNumber { get; set; }
        public string? LoanDeviceEmail { get; set; }
        public Guid CustomerId { get; set; }
    }
}
