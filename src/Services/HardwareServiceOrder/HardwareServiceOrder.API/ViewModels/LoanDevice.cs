namespace HardwareServiceOrder.API.ViewModels
{
    public class LoanDevice
    {
        public string? PhoneNumber { get; set; }
        [RequiredIfProvidesLoanDevice]
        public string? Email { get; set; }
        public bool ProvidesLoanDevice { get; set; }
        public Guid CallerId { get; set; }
        public LoanDevice(string phone, string email, bool providesLoanDevice = false)
        {
            PhoneNumber = phone;
            Email = email;
            ProvidesLoanDevice = providesLoanDevice;
        }
        public LoanDevice()
        {

        }
    }

    /// <summary>
    /// Validation rules for loan device email and phone number
    /// </summary>
    public class RequiredIfProvidesLoanDeviceAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var loanDevice = (LoanDevice)validationContext.ObjectInstance;
            var email = value as string;

            if (loanDevice.ProvidesLoanDevice && !string.IsNullOrWhiteSpace(email))
                return ValidationResult.Success;

            if (!loanDevice.ProvidesLoanDevice && string.IsNullOrWhiteSpace(email) && string.IsNullOrEmpty(loanDevice.PhoneNumber))
                return ValidationResult.Success;

            if (!loanDevice.ProvidesLoanDevice && (!string.IsNullOrWhiteSpace(email) || !string.IsNullOrWhiteSpace(loanDevice.PhoneNumber)))
                return new ValidationResult("Both email and phone number should be empty.");

            return new ValidationResult("Email is required.");
        }
    }
}
