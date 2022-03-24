using System.ComponentModel.DataAnnotations;

namespace Customer.API.ViewModels
{
    public record ContactPerson
    {
        public ContactPerson(CustomerServices.Models.ContactPerson customerContactPerson)
        {
            FirstName = (customerContactPerson == null) ? "" : customerContactPerson.FirstName;
            LastName = (customerContactPerson == null) ? "" : customerContactPerson.LastName;
            Email = (customerContactPerson == null) ? "" : customerContactPerson.Email;
            PhoneNumber = (customerContactPerson == null) ? "" : customerContactPerson.PhoneNumber;
        }

        public ContactPerson(){}

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}