using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class CustomerStandardBusinessSubscriptionProduct : Entity
    {
        public CustomerStandardBusinessSubscriptionProduct()
        {
        }

        public CustomerStandardBusinessSubscriptionProduct(string? dataPackage, string subscriptionName, Guid callerId)
        {
            DataPackage = dataPackage;
            SubscriptionName = subscriptionName;
            CreatedBy = callerId;


        }
        public string SubscriptionName { get; set; }
        public string? DataPackage { get; set; }
        public IReadOnlyCollection<SubscriptionAddOnProduct>? AddOnProducts { get; set; }


    }

}
