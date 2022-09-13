
namespace HardwareServiceOrder.API.ViewModels
{
    /// <summary>
    ///     An API write-model for creating or updating an organization's <c><see cref="CustomerSettings"/></c>.
    /// </summary>
    public class NewCustomerSettings : IValidatableObject
    {
        /// <summary>
        ///     Does the organization offer loan-devices to their employee when their regular device is sent on repairs?
        ///     
        ///     <para>
        ///     If this is set to <see langword="true"/>, a value for either <see cref="LoanDevicePhoneNumber"/> 
        ///     or <see cref="LoanDeviceEmail"/> is required. </para>
        /// </summary>
        [Required]
        public bool ProvidesLoanDevice { get; set; }


        /// <summary> Backing field for <see cref="LoanDevicePhoneNumber"/>. </summary>
        private string? _loanDevicePhoneNumber;

        /// <summary>
        ///     If the company <see cref="ProvidesLoanDevice">offers loan-devices</see>, their employees can contact them using
        ///     this phone-number if they need to obtain a loan-device. This will be <see langword="null"/> if the company
        ///     don't offer contact over phone.
        /// </summary>
        /// <remarks>
        ///     The phone-number must be in <c>E.164</c> format.
        /// </remarks>
        [Phone]
        [StringLength(maximumLength: 15)]
        [RegularExpression("^\\+[1-9]\\d{1,14}$")]
        public string? LoanDevicePhoneNumber
        {
            get { return _loanDevicePhoneNumber; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    _loanDevicePhoneNumber = null;
                else
                    _loanDevicePhoneNumber = value?.Trim().ToLowerInvariant();
            }
        }


        /// <summary> Backing field for <see cref="LoanDeviceEmail"/>. </summary>
        private string? _loanDeviceEmail;

        /// <summary>
        ///     If the company <see cref="ProvidesLoanDevice">offers loan-devices</see>, their employees can contact them on this e-mail
        ///     if they need to obtain a loan-device. This will be <see langword="null"/> if the company don't offer contact over e-mail.
        /// </summary>
        [EmailAddress]
        [StringLength(maximumLength: 320)]
        public string? LoanDeviceEmail
        {
            get { return _loanDeviceEmail; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    _loanDeviceEmail = null;
                else
                    _loanDeviceEmail = value?.Trim().ToLowerInvariant();
            }
        }


        /// <inheritdoc/>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // If "ProvidesLoanDevice == true", then at least one of the email/phone properties must be provided.
            if (ProvidesLoanDevice && string.IsNullOrWhiteSpace(LoanDeviceEmail) && string.IsNullOrWhiteSpace(LoanDevicePhoneNumber))
            {
                yield return new ValidationResult("The configuration has enabled loan-devices, but no contact alternatives exist. Add at least one contact alternative.",
                                                  new[] { nameof(ProvidesLoanDevice), nameof(LoanDeviceEmail), nameof(LoanDevicePhoneNumber) });
            }
        }
    }
}
