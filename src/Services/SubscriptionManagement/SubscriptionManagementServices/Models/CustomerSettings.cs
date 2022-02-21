using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class CustomerSettings : Entity, IAggregateRoot
    {

        public CustomerSettings(Guid customerId, ICollection<CustomerOperatorSettings>? customerOperatorSettings = null, IReadOnlyCollection<CustomerReferenceField>? customerReferenceFields = null)
        {
            CustomerId = customerId;
            if (customerOperatorSettings != null)
            {
                _customerOperatorSettings.AddRange(customerOperatorSettings);
            }

            if (customerReferenceFields != null)
            {
                _customerReferenceFields.AddRange(customerReferenceFields);
            }
        }

        public CustomerSettings(){}
        
        public CustomerSettings(Guid customerId)
        {
            CustomerId = customerId;
            _customerOperatorSettings = new List<CustomerOperatorSettings>();
        }

        public Guid CustomerId { get; protected set; }
        private readonly List<CustomerOperatorSettings> _customerOperatorSettings = new();
        public IReadOnlyCollection<CustomerOperatorSettings> CustomerOperatorSettings => _customerOperatorSettings.AsReadOnly();

        private readonly List<CustomerReferenceField> _customerReferenceFields = new();
        public IReadOnlyCollection<CustomerReferenceField>? CustomerReferenceFields => _customerReferenceFields.AsReadOnly();

        public void AddCustomerReferenceField(CustomerReferenceField customerReferenceField)
        {
            if (!_customerReferenceFields.Any(r => r.Name == customerReferenceField.Name && r.ReferenceType == customerReferenceField.ReferenceType))
            {
                _customerReferenceFields.Add(customerReferenceField);
            }
        }

        public void AddCustomerOperatorSettings(List<CustomerOperatorSettings> customerOperatorSettings)
        {
            _customerOperatorSettings.AddRange(customerOperatorSettings);
        }

        public void RemoveCustomerOperatorSettings(int operatorId)
        {
            var customerOperatorSettings = _customerOperatorSettings.FirstOrDefault(x => x.Operator.Id == operatorId);

            if (customerOperatorSettings != null)
            {
                _customerOperatorSettings.Remove(customerOperatorSettings);
                return;
            }

            throw new ArgumentException($"Operator {operatorId} is not associated with this customer.");
        }

        public CustomerSubscriptionProduct AddSubscriptionProductAsync(Operator @operator, string productName, IList<string>? selectedDataPackages, SubscriptionProduct? globalSubscriptionProduct, Guid callerId)
        {
            //If the operator settings is null then add operator to customer
            var customerOperatorSetting = CustomerOperatorSettings.FirstOrDefault(os => os.Operator.Id == @operator.Id);
            if (customerOperatorSetting == null)
            {
                customerOperatorSetting = new CustomerOperatorSettings(@operator, null);
                _customerOperatorSettings.Add(customerOperatorSetting);
            }

            var foundSubscriptionProduct = customerOperatorSetting.AvailableSubscriptionProducts.FirstOrDefault(p => p.SubscriptionName == productName);
            if (foundSubscriptionProduct == null)
            {
               
               var dataPackages = selectedDataPackages?.Select(dataPackage => new DataPackage(dataPackage, callerId)).ToList();
                var globalDataPackages = new List<DataPackage>();
                if (globalSubscriptionProduct?.DataPackages.Count != null) {
                    foreach (var dataPackage in selectedDataPackages)
                    {
                        var exsitingDatapackages = globalSubscriptionProduct.DataPackages.FirstOrDefault(a => a.DataPackageName == dataPackage);
                        if (exsitingDatapackages != null)
                        {
                            globalDataPackages.Add(exsitingDatapackages);
                        }
                    }
                }
                var customerSubscriptionProduct = globalSubscriptionProduct != null
                    ? new CustomerSubscriptionProduct(globalSubscriptionProduct, callerId, globalDataPackages)
                    : new CustomerSubscriptionProduct(productName, @operator, callerId, dataPackages);
                foundSubscriptionProduct = customerSubscriptionProduct;
                customerOperatorSetting.AvailableSubscriptionProducts.Add(customerSubscriptionProduct);
            }
            else
            {
                foundSubscriptionProduct.SubscriptionName = globalSubscriptionProduct != null ? globalSubscriptionProduct.SubscriptionName : productName;


                foundSubscriptionProduct.SetDataPackages(globalSubscriptionProduct?.DataPackages, selectedDataPackages, callerId);
                
                foundSubscriptionProduct.GlobalSubscriptionProduct = globalSubscriptionProduct;
            }

            return foundSubscriptionProduct;
        }

        public CustomerReferenceField? RemoveCustomerReferenceField(int customerReferenceFieldId)
        {
            if (CustomerReferenceFields == null)
            {
                return null;
            }

            var customerReferenceField = _customerReferenceFields.FirstOrDefault(r => r.Id == customerReferenceFieldId);
            if (customerReferenceField == null)
            {
                return null;
            }
            _customerReferenceFields.Remove(customerReferenceField);
            return customerReferenceField;
        }
    }
}
