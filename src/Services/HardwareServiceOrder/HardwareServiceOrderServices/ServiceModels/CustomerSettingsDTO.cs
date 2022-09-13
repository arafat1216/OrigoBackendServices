namespace HardwareServiceOrderServices.ServiceModels
{
    /// <inheritdoc cref="Models.CustomerSettings"/>
    public class CustomerSettingsDTO
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="CustomerSettingsDTO"/> class.
        ///     
        ///     <para>
        ///     This is a reserved constructor intended for AutoMapper, unit-testing and other automated processes. This constructor should
        ///     never be called directly in any production code. </para>
        /// </summary>
        public CustomerSettingsDTO()
        {
        }


        /// <summary>
        ///     Creates a new instance of the <see cref="CustomerSettingsDTO"/> class.
        /// </summary>
        public CustomerSettingsDTO(Guid organizationId, bool providesLoanDevice, string? loanDevicePhoneNumber, string? loanDeviceEmail)
        {
            CustomerId = organizationId;
            ProvidesLoanDevice = providesLoanDevice;
            LoanDevicePhoneNumber = loanDevicePhoneNumber;
            LoanDeviceEmail = loanDeviceEmail;
        }


        /// <inheritdoc cref="Models.CustomerSettings.CustomerId"/>
        [Required]
        public Guid CustomerId { get; set; }


        /// <inheritdoc cref="Models.CustomerSettings.ProvidesLoanDevice"/>
        public bool ProvidesLoanDevice { get; set; }


        /// <inheritdoc cref="Models.CustomerSettings.LoanDevicePhoneNumber"/>
        [Phone]
        [StringLength(maximumLength: 15)]
        [RegularExpression("^\\+[1-9]\\d{1,14}$")]
        public string? LoanDevicePhoneNumber { get; set; }


        /// <summary> Backing field for <see cref="LoanDeviceEmail"/>. </summary>
        private string? _loanDeviceEmail { get; set; }

        /// <inheritdoc cref="Models.CustomerSettings.LoanDeviceEmail"/>
        [EmailAddress]
        [StringLength(maximumLength: 320)]
        public string? LoanDeviceEmail
        {
            get { return _loanDeviceEmail; }
            set { _loanDeviceEmail = value?.ToLowerInvariant(); }
        }



    }
}
