namespace Customer.API.ViewModels
{
    public record Address
    {
        public Address(CustomerServices.Models.Address customerCompanyAddress)
        {
            Street = (customerCompanyAddress == null) ? "" : customerCompanyAddress.Street;
            PostCode = (customerCompanyAddress == null) ? "" : customerCompanyAddress.PostCode;
            City = (customerCompanyAddress == null) ? "" : customerCompanyAddress.City;
            Country = (customerCompanyAddress == null) ? "" : customerCompanyAddress.Country;
        }

        public Address(){}

        public string Street { get; set; }

        public string PostCode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
    }
}