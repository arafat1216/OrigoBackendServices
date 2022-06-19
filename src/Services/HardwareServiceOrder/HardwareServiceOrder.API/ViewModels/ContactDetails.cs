namespace HardwareServiceOrder.API.ViewModels
{
    /// <summary>
    ///     Represents the contact information for a single user or service-order owner.
    /// </summary>
    public class ContactDetails
    {
        /// <inheritdoc cref="ContactDetailsDTO.UserId"/>
        [Required]
        public Guid UserId { get; set; }

        /// <inheritdoc cref="ContactDetailsDTO.FirstName"/>
        [Required]
        public string FirstName { get; set; }

        /// <inheritdoc cref="ContactDetailsDTO.LastName"/>
        [Required]
        public string LastName { get; set; }

        /// <inheritdoc cref="ContactDetailsDTO.Email"/>
        [Required]
        [MaxLength(320)]
        public string Email { get; set; }

        /// <inheritdoc cref="ContactDetailsDTO.PhoneNumber"/>
        [Phone]
        [RegularExpression("^\\+[1-9]\\d{1,14}$")]
        [StringLength(maximumLength: 15)] // The longest possible length for a valid E.164 phone-number
        public string? PhoneNumber { get; set; }
    }
}