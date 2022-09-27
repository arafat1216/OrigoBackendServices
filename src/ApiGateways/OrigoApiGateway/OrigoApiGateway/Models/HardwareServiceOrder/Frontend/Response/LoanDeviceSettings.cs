using OrigoApiGateway.Models.HardwareServiceOrder.Backend;

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response
{
    /// <summary>
    ///     An API view-model that contains a subset of the organization's <see cref="CustomerSettings"/>, detailing how they handle loan-devices.
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public class LoanDeviceSettings
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
