namespace Customer.API.ViewModels
{
    public record ContactPerson
    {
        public ContactPerson(CustomerServices.Models.ContactPerson customerContactPerson)
        {
            FullName = (customerContactPerson == null) ? "" : customerContactPerson.FullName;
            Email = (customerContactPerson == null) ? "" : customerContactPerson.Email;
            PhoneNumber = (customerContactPerson == null) ? "" : customerContactPerson.PhoneNumber;
        }

        public ContactPerson(){}

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

    }
}