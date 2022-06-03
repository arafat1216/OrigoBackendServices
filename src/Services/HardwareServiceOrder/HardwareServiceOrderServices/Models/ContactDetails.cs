using System.ComponentModel.DataAnnotations;

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
        ///     The email-address that should be used when sending emails to the contact.
        /// </summary>
        /// <remarks>
        ///     The maximum total length for an email address is 320 characters.
        /// </remarks>
        [MaxLength(320)]
        [EmailAddress]
        public string Email { get; set; }


        /// <summary>
        ///     Constructor reserved for Entity Framework
        /// </summary>
        private ContactDetails()
        {
        }


        public ContactDetails(Guid userId, string firstName, string email)
        {
            UserId = userId;
            FirstName = firstName;
            Email = email;
        }
    }
}
