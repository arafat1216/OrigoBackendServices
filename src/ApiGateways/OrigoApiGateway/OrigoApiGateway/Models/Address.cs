using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Models
{
    public class Address
    {
        public Address(AddressDTO companyAddress)
        {
            Street = companyAddress.Street;
            Postcode = companyAddress.Postcode;
            City = companyAddress.City;
            Country = companyAddress.Country;
        }

        public Address(){}

        public string Street { get; set; }

        public string Postcode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
    }
}