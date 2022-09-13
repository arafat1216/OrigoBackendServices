using Common.Seedwork;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents the global service-settings for a single customer (organization).
    /// </summary>
    public class CustomerSettings : EntityV2, IAggregateRoot, IDbSetEntity
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="CustomerSettings"/> class.
        ///     
        ///     <para>
        ///     This is a reserved constructor intended for Entity Framework, AutoMapper, unit-testing and other automated processes.
        ///     This constructor should never be called directly in any production code. </para>
        /// </summary>
        private CustomerSettings()
        {
        }


        [Obsolete("This is due to be removed.")]
        public CustomerSettings(Guid organizationId)
        {
            CustomerId = organizationId;
            ProvidesLoanDevice = false;
        }


        /// <summary>
        ///     Creates a new instance of the <see cref="CustomerSettings"/> class.
        /// </summary>
        /// <param name="organizationId"> The customer's organization ID </param>
        /// <param name="providesLoanDevice"> Does the organization offer loan-devices to their employee when their regular device is sent on repairs? </param>
        /// <param name="loanPhoneNumber"> The contact phone-number that should be used if users needs a loan-device. </param>
        /// <param name="loanPhoneEmail"> The contact-email that should be used if users needs a loan-device. </param>
        public CustomerSettings(Guid organizationId, bool providesLoanDevice, string? loanPhoneNumber, string? loanPhoneEmail)
        {
            CustomerId = organizationId;
            ProvidesLoanDevice = providesLoanDevice;
            LoanDeviceEmail = loanPhoneEmail;
            LoanDevicePhoneNumber = loanPhoneNumber;
        }


        /// <summary>
        ///     The customer's organization ID
        /// </summary>
        public Guid CustomerId { get; set; }


        /// <summary>
        ///     Does the organization offer loan-devices to their employee when their regular device is sent on repairs?
        ///     
        ///     <para>
        ///     If this is set to <see langword="true"/>, a value for either <see cref="LoanDevicePhoneNumber"/> 
        ///     or <see cref="LoanDeviceEmail"/> is required. </para>
        /// </summary>
        public bool ProvidesLoanDevice { get; set; }


        /// <summary>
        ///     If the company <see cref="ProvidesLoanDevice">offers loan-devices</see>, their employees can contact them using
        ///     this phone-number if they need to obtain a loan-device. This will be <see langword="null"/> if the company
        ///     don't offer contact over phone.
        /// </summary>
        /// <remarks>
        ///     The phone-number must be in <c>E.164</c> format.
        /// </remarks>
        [Phone]
        [RegularExpression("^\\+[1-9]\\d{1,14}$")]
        [StringLength(maximumLength: 15)] // The longest possible length for a valid E.164 phone-number
        public string? LoanDevicePhoneNumber { get; set; }


        /// <summary>
        ///     If the company <see cref="ProvidesLoanDevice">offers loan-devices</see>, their employees can contact them on this e-mail
        ///     if they need to obtain a loan-device. This will be <see langword="null"/> if the company don't offer contact over e-mail.
        /// </summary>
        [EmailAddress]
        [StringLength(maximumLength: 320)] // The RFC's max-length for email addresses
        public string? LoanDeviceEmail { get; set; }

    }
}
