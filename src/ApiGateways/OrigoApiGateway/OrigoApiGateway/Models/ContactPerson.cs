using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Models
{
    public class ContactPerson
    {
        public ContactPerson(ContactPersonDTO customerContactPerson)
        {
            FirstName = customerContactPerson.FirstName;
            LastName = customerContactPerson.LastName;
            Email = customerContactPerson.Email;
            PhoneNumber = customerContactPerson.PhoneNumber;
        }

        public ContactPerson(){}

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}