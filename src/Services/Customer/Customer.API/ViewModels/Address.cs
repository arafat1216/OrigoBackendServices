namespace Customer.API.ViewModels
{
    public record Address
    {
        public Address(CustomerServices.Models.Address customerCompanyAddress)
        {
            Street = customerCompanyAddress.Street;
            PostCode = customerCompanyAddress.PostCode;
            City = customerCompanyAddress.City;
            Country = customerCompanyAddress.Country;
        }

        public Address(){}

        public string Street { get; set; }

        public string PostCode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
    }
}