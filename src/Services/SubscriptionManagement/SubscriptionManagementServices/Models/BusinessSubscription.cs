using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class BusinessSubscription : Entity
    {
        public BusinessSubscription()
        {

        }

        public BusinessSubscription(string name, string organizationNumber, string address, string postalCode, string postalPlace, string country)
        {
            Name = name;
            OranizationNumber = organizationNumber;
            Address = address;
            PostalCode = postalCode;
            PostalPlace = postalPlace;
            Country = country;
        }

        public string Name { get; set; }
        public string OranizationNumber { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PostalPlace { get; set; }
        public string Country { get; set; }
    }
}
