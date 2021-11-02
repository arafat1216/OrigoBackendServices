using System.Collections.Generic;
using Common.Seedwork;
using Microsoft.EntityFrameworkCore;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace CustomerServices.Models
{
    [Owned]
    public class Address : ValueObject
    {
        public string Street { get; private set; }
        public string PostCode { get; private set; }

        public string City { get; private set; }
        public string Country { get; private set; }

        public Address() { }

        public Address(string street, string postCode, string city, string country)
        {
            Street = (street == null) ? "" : street;
            PostCode = (postCode == null) ? "" : postCode;
            City = (city == null) ? "" : city;
            Country = (country == null) ? "" : country;
        }

        public void PatchAddress(string street, string postCode, string city, string country)
        {
            if (street != null)
                Street = street;
            if (postCode != null)
                PostCode = postCode;
            if (city != null)
                City = city;
            if (country != null)
                Country = country;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Street;
            yield return City;
            yield return Country;
            yield return PostCode;
        }
    }
}
