﻿namespace OrigoApiGateway.Models
{
    public class OrigoAddress
    {
        public OrigoAddress(AddressDTO companyAddress)
        {
            Street = companyAddress.Street;
            Postcode = companyAddress.Postcode;
            City = companyAddress.City;
            Country = companyAddress.Country;
        }

        public OrigoAddress(){}

        public string Street { get; set; }

        public string Postcode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
    }
}