
using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class SimCardAddress : Entity
    {
        public SimCardAddress()
        {
        }

        public SimCardAddress(string? firstName, string? lastName, string? address, string? postalCode, string? postalPlace, string? country)
        {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            PostalCode = postalCode;
            PostalPlace = postalPlace;
            Country = country;
        }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? PostalPlace { get; set; }
        public string? Country { get; set; }
    }
}
