using Common.Seedwork;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.Models
{
    public class PrivateSubscription: Entity
    {
        public PrivateSubscription()
        {

        }

        public PrivateSubscription(string firstName, string lastName, string address, string postalCode, string postalPlace, string country, string email, DateTime dob, string operatorName, PrivateSubscription? realOwner)
        {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            PostalCode = postalCode;    
            PostalPlace = postalPlace;
            Country = country;
            Email = email;
            BirthDate = dob;
            OperatorName = operatorName;
            RealOwner = realOwner;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PostalPlace { get; set; }
        [MaxLength(2)]
        public string Country { get; set; }
        [MaxLength(320)]
        public string? Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string OperatorName { get; set; }
        public PrivateSubscription? RealOwner { get; set; }
    }
}
