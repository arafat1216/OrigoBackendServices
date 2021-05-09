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

        public Address(string street, string city, string country, string postCode)
        {
            Street = street;
            City = city;
            Country = country;
            PostCode = postCode;
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
