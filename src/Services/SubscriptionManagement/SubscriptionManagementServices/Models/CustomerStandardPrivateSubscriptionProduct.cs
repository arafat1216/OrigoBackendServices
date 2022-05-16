using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class CustomerStandardPrivateSubscriptionProduct : Entity
    {
        public CustomerStandardPrivateSubscriptionProduct()
        {
        }

        public CustomerStandardPrivateSubscriptionProduct(string? dataPackage, string subscriptionName, Guid callerId)
        {
            DataPackage = dataPackage;
            SubscriptionName = subscriptionName;
            CreatedBy = callerId;
            

        }
        public string SubscriptionName { get; set; }
        public string? DataPackage { get; set; }


    }
}
