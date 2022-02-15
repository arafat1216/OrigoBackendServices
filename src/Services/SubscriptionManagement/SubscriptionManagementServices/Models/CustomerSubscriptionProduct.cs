using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManagementServices.Models
{
    public class CustomerSubscriptionProduct : SubscriptionProduct
    {
        public CustomerSubscriptionProduct()
        {
        }
        public CustomerSubscriptionProduct(string subscriptionName, int operatorId, IList<DataPackage>? dataPackages, Guid callerId)
        {
            SubscriptionName = subscriptionName;
            OperatorId = operatorId;
            DataPackages = new ReadOnlyCollection<DataPackage>(dataPackages ?? new List<DataPackage>());
            CreatedBy = callerId;
            UpdatedBy = callerId;
        }

        public CustomerSubscriptionProduct(SubscriptionProduct? globalSubscriptionProduct)
        {
            GlobalSubscriptionProduct = globalSubscriptionProduct;
            Name = globalSubscriptionProduct.SubscriptionName;
        }

        public string Name { get; set; }

        public SubscriptionProduct? GlobalSubscriptionProduct { get; set; }
    }
}
