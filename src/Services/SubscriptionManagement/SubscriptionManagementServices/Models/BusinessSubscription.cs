using Common.Seedwork;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.Models
{
    public class BusinessSubscription : Entity
    {
        public BusinessSubscription()
        {

        }

        public BusinessSubscription(string name, string organizationNumber, string address, string postalCode, string postalPlace, string country, string contactPerson)
        {
            Name = name;
            OrganizationNumber = organizationNumber;
            Address = address;
            PostalCode = postalCode;
            PostalPlace = postalPlace;
            Country = country;
            ContactPerson = contactPerson;
        }

        public string Name { get; set; }
        public string OrganizationNumber { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PostalPlace { get; set; }
        [MaxLength(2)]
        public string Country { get; set; }
        public string? ContactPerson { get; set; }
    }
}
