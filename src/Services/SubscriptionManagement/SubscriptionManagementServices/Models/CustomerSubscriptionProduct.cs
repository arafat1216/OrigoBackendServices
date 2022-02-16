using System.Collections.ObjectModel;
using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class CustomerSubscriptionProduct : Entity
    {
        public CustomerSubscriptionProduct()
        {
        }
        public CustomerSubscriptionProduct(string subscriptionName, Operator @operator, Guid callerId, IList<DataPackage>? dataPackages)
        {
            SubscriptionName = subscriptionName;
            Operator = @operator;
            if (dataPackages != null) _dataPackages.AddRange(dataPackages);
        }

        public CustomerSubscriptionProduct(SubscriptionProduct globalSubscriptionProduct, Guid callerId, IList<DataPackage>? dataPackages)
        {
            GlobalSubscriptionProduct = globalSubscriptionProduct;
            SubscriptionName = globalSubscriptionProduct.SubscriptionName;
            Operator = globalSubscriptionProduct.Operator;
            if (dataPackages != null) _dataPackages.AddRange(dataPackages);
        }

        public string SubscriptionName { get; set; }
        public Operator Operator { get; set; }

        private List<DataPackage> _dataPackages = new();
        public ICollection<DataPackage> DataPackages => _dataPackages.AsReadOnly();

        public void SetDataPackages(IList<string> dataPackages, Guid callerId)
        {
            _dataPackages = new List<DataPackage>();
            foreach (var dataPackageName in dataPackages)
            {
                _dataPackages.Add(new DataPackage(dataPackageName, callerId));
            }
        }


        public SubscriptionProduct? GlobalSubscriptionProduct { get; set; }

        public ICollection<SubscriptionOrder>? SubscriptionOrders { get; set; }
    }
}
