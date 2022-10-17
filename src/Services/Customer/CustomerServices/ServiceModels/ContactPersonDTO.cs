using CustomerServices.Models;
using System.ComponentModel.DataAnnotations;

namespace CustomerServices.ServiceModels
{
    public record ContactPersonDTO
    {
        public ContactPersonDTO(ContactPerson customerContactPerson)
        {
            FirstName = (customerContactPerson == null) ? "" : customerContactPerson.FirstName;
            LastName = (customerContactPerson == null) ? "" : customerContactPerson.LastName;
            Email = (customerContactPerson == null) ? "" : customerContactPerson.Email;
            PhoneNumber = (customerContactPerson == null) ? "" : customerContactPerson.PhoneNumber;
        }

        public ContactPersonDTO() { }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress]
        [MaxLength(2)]
        public string Email { get; set; }

        /// <summary>
        /// The persons phone-number, standardized using the <c>E.164</c> format.
        /// </summary>
        /// <example>+4712345678</example>
        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
    }
}