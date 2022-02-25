using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class BusinessSubscription : Entity
    {
        public string Name { get; set; }
        public string OranizationNumber { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PostalPlace { get; set; }
        public string Country { get; set; }
    }
}
