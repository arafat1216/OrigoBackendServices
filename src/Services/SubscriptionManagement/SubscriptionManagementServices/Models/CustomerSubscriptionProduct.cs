using System.Collections.ObjectModel;

namespace SubscriptionManagementServices.Models
{
    public class CustomerSubscriptionProduct : SubscriptionProduct
    {
        public CustomerSubscriptionProduct()
        {
        }
        public CustomerSubscriptionProduct(string subscriptionName, Operator @operator, Guid callerId, IList<DataPackage> dataPackages)
        {
            SubscriptionName = subscriptionName;
            OperatorId = @operator.Id;
            Operator = @operator;
            CreatedBy = callerId;
            UpdatedBy = callerId;
            Name = subscriptionName;
            DataPackages = new ReadOnlyCollection<DataPackage>(dataPackages ?? new List<DataPackage>());
            //GlobalSubscriptionProduct = isGlobal;
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
