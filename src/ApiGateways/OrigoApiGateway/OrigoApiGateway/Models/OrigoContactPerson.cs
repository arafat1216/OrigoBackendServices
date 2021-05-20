namespace OrigoApiGateway.Models
{
    public class OrigoContactPerson
    {
        public OrigoContactPerson(ContactPersonDTO customerContactPerson)
        {
            FullName = customerContactPerson.FullName;
            Email = customerContactPerson.Email;
            PhoneNumber = customerContactPerson.PhoneNumber;
        }

        public OrigoContactPerson(){}

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}