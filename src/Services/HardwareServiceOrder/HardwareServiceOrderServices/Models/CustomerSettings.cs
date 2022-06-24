using Common.Seedwork;
using System.ComponentModel.DataAnnotations;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents the global service-settings for a single customer (organization).
    /// </summary>
    public class CustomerSettings : EntityV2, IAggregateRoot
    {
        public CustomerSettings(Guid customerId, Guid callerId)
        {
            CustomerId = customerId;
            //CreatedBy = callerId;
        }

        public CustomerSettings()
        {

        }

        public CustomerSettings(Guid customerId, string loanPhoneNumber, string loanPhoneEmail,  bool providesLoanDevice, Guid callerId)
        {
            CustomerId = customerId;
            LoanDeviceEmail = loanPhoneEmail;
            LoanDevicePhoneNumber = loanPhoneNumber;
            ProvidesLoanDevice = providesLoanDevice;
        }

        /// <summary>
        ///     The phone-number in <c>E.164</c> format.
        /// </summary>
        [Phone]
        [RegularExpression("^\\+[1-9]\\d{1,14}$")]
        [StringLength(maximumLength: 15)] // The longest possible length for a valid E.164 phone-number
        public string? LoanDevicePhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(maximumLength: 320)] // The RFC's max-length for email addresses
        public string? LoanDeviceEmail { get; set; }

        /// <summary>
        /// Customer Identifier
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// This property ensures whether a customer provides loan device
        /// </summary>
        public bool ProvidesLoanDevice { get; set; }
    }
}
