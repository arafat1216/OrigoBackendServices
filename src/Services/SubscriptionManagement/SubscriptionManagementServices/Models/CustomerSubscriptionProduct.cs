namespace SubscriptionManagementServices.Models
{
    public class CustomerSubscriptionProduct : SubscriptionProduct
    {
        public CustomerSubscriptionProduct()
        {
        }
        public CustomerSubscriptionProduct(string subscriptionName, int operatorId, Guid callerId)
        {
            SubscriptionName = subscriptionName;
            OperatorId = operatorId;
            CreatedBy = callerId;
            UpdatedBy = callerId;
            Name = subscriptionName;
        }

        public CustomerSubscriptionProduct(SubscriptionProduct? globalSubscriptionProduct)
        {
            GlobalSubscriptionProduct = globalSubscriptionProduct;
            Name = globalSubscriptionProduct.SubscriptionName;
            SubscriptionName = globalSubscriptionProduct.SubscriptionName;
            OperatorId = globalSubscriptionProduct.OperatorId;
            CreatedBy = globalSubscriptionProduct.CreatedBy;
            UpdatedBy = globalSubscriptionProduct.UpdatedBy;
        }

        public string Name { get; set; }

        public SubscriptionProduct? GlobalSubscriptionProduct { get; set; }
    }
}
