using CustomerServices.ServiceModels;

namespace Customer.API.ViewModels
{
    public record NewCustomer
    {
        public string CompanyName { get; set; }

        public string OrgNumber { get; set; }

        public AddressDTO CompanyAddress { get; set; }

        public ContactPerson CustomerContactPerson { get; set; }
    }
}