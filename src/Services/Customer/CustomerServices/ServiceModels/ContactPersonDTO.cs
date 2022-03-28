using System.ComponentModel.DataAnnotations;

namespace CustomerServices.ServiceModels
{
    public record ContactPersonDTO
    {
        public ContactPersonDTO(CustomerServices.Models.ContactPerson customerContactPerson)
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
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}