namespace HardwareServiceOrderServices.Models
{
    public class ContactDetails
    {
        /// <summary>
        ///     The user-ID for this contact.
        /// </summary>
        public Guid UserId { get; init; }

        /// <summary>
        ///     The name of the contact that should be used for email and other contact information.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     The last name of the person that handles the service-order.
        /// </summary>
        /// <example> Doe </example>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        ///     The email-address that should be used when sending emails to the contact.
        /// </summary>
        /// <remarks>
        ///     The maximum total length for an email address is 320 characters.
        /// </remarks>
        [MaxLength(320)]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        ///     An phone-number in <c>E.164</c> format that the service-provider can use to get in touch with the person that handles the service-order.
        /// </summary>
        [Phone]
        [RegularExpression("^\\+[1-9]\\d{1,14}$")]
        [StringLength(maximumLength: 15)] // The longest possible length for a valid E.164 phone-number
        public string? PhoneNumber { get; set; }



        /// <summary>
        ///     Constructor reserved for Entity Framework.
        /// </summary>
        private ContactDetails()
        {
        }


        public ContactDetails(Guid userId, string firstName, string lastName, string email, string? phoneNumber)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }
    }
}
