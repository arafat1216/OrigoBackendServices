using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Models
{
    public class ContactPerson
    {
        public ContactPerson(ContactPersonDTO customerContactPerson)
        {
            FullName = customerContactPerson.FullName;
            Email = customerContactPerson.Email;
            PhoneNumber = customerContactPerson.PhoneNumber;
        }

        public ContactPerson(){}

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}