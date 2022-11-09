using Common.Seedwork;
using System.Text.Json.Serialization;

namespace SubscriptionManagementServices.Models
{
    public class CustomerStandardBusinessSubscriptionProduct : Entity
    {
        public CustomerStandardBusinessSubscriptionProduct()
        {
        }

        public CustomerStandardBusinessSubscriptionProduct(string? dataPackage, string subscriptionName, Guid callerId, List<SubscriptionAddOnProduct>? subscriptionAddOnProducts)
        {
            DataPackage = dataPackage;
            SubscriptionName = subscriptionName;
            CreatedBy = callerId;
            _addOnProducts = subscriptionAddOnProducts;

        }
        public string SubscriptionName { get; set; }
        public string? DataPackage { get; set; }
        [JsonIgnore]
        public IReadOnlyCollection<SubscriptionAddOnProduct>? AddOnProducts => _addOnProducts.AsReadOnly();

        private readonly List<SubscriptionAddOnProduct> _addOnProducts = new();


    }

}
