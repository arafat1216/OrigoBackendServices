using System.Collections.Generic;
using Common.Seedwork;

namespace CustomerServices.Models
{
    public class Address : ValueObject
    {
        public string Street { get; }
        public string PostCode { get; }

        public string City { get; }
        public string Country { get; }

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
