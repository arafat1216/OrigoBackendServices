using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Common.Seedwork;
using SubscriptionManagementServices.DomainEvents;

namespace SubscriptionManagementServices.Models
{
    public class CustomerSubscriptionProduct : Entity
    {
        public CustomerSubscriptionProduct()
        {
        }

        /// <summary>
        /// Used for testing to be able to set identity id.
        /// </summary>
        public CustomerSubscriptionProduct(int id, string subscriptionName, Operator @operator, Guid callerId, IList<DataPackage>? dataPackages)
        : this(subscriptionName, @operator, callerId, dataPackages)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Id = id;
        }

        public CustomerSubscriptionProduct(string subscriptionName, Operator @operator, Guid callerId, IList<DataPackage>? dataPackages)
        {
            SubscriptionName = subscriptionName;
            Operator = @operator;
            if (dataPackages != null) _dataPackages.AddRange(dataPackages);
            
        }

        /// <summary>
        /// Used for testing to be able to set identity id.
        /// </summary>
        public CustomerSubscriptionProduct(int id, SubscriptionProduct globalSubscriptionProduct, Guid callerId, IList<DataPackage>? dataPackages)
            : this(globalSubscriptionProduct, callerId, dataPackages)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Id = id;
        }

        public CustomerSubscriptionProduct(SubscriptionProduct globalSubscriptionProduct, Guid callerId, IList<DataPackage>? dataPackages)
        {
            GlobalSubscriptionProduct = globalSubscriptionProduct;
            SubscriptionName = globalSubscriptionProduct.SubscriptionName;
            Operator = globalSubscriptionProduct.Operator;
            if (dataPackages != null) _dataPackages.AddRange(dataPackages);
           
        }

        public string SubscriptionName { get; set; }

        [JsonIgnore]
        public Operator Operator { get; set; }

        public CustomerOperatorSettings CustomerOperatorSettings { get; set; }

        private readonly List<DataPackage> _dataPackages = new();

        [JsonIgnore]
        public ICollection<DataPackage> DataPackages => _dataPackages.AsReadOnly();

        public void SetDataPackages(ICollection<DataPackage>? globalDataPackages, IEnumerable<string>? selectedDataPackages, Guid callerId)
        {
            _dataPackages.Clear();
            if (selectedDataPackages == null) return;
            foreach (var selectedDataPackage in selectedDataPackages)
            {
                if (globalDataPackages == null) continue;
                var dataPackage = globalDataPackages.FirstOrDefault(a => a.DataPackageName == selectedDataPackage);
                if (dataPackage != null)
                {
                    _dataPackages.Add(dataPackage);
                }
            }
        }

        [JsonIgnore]
        public SubscriptionProduct? GlobalSubscriptionProduct { get; set; }
    }
}
