using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class SubscriptionAddOnProduct : Entity
    {
        public SubscriptionAddOnProduct()
        {
        }

        public SubscriptionAddOnProduct(string addOnProductName, Guid callerId, SubscriptionProduct belongsToSubscriptionProduct)
        {
            AddOnProductName = addOnProductName;
            BelongsToSubscriptionProduct = belongsToSubscriptionProduct;
            UpdatedBy = callerId;
            CreatedBy = callerId;
        }

        public string AddOnProductName { get; set; }

        public SubscriptionProduct BelongsToSubscriptionProduct { get; set; }

        public IReadOnlyCollection<SubscriptionOrder>? SubscriptionOrders { get; set; }

    }
}
