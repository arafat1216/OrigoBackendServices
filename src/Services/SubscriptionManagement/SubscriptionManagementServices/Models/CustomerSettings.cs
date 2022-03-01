using Common.Seedwork;
using SubscriptionManagementServices.DomainEvents;
using System.Text.Json.Serialization;
using SubscriptionManagementServices.ServiceModels;

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
        
        public CustomerSettings(Guid customerId, Guid callerId)
        {
            CustomerId = customerId;
            _customerOperatorSettings = new List<CustomerOperatorSettings>();
            AddDomainEvent(new CustomerSettingsCreatedDomainEvent<CustomerSettings>(this, callerId));
        }

        public Guid CustomerId { get; protected set; }

        private readonly List<CustomerOperatorSettings> _customerOperatorSettings = new();
        [JsonIgnore]
        public IReadOnlyCollection<CustomerOperatorSettings> CustomerOperatorSettings => _customerOperatorSettings.AsReadOnly();

        private readonly List<CustomerReferenceField> _customerReferenceFields = new();
        [JsonIgnore]
        public IReadOnlyCollection<CustomerReferenceField>? CustomerReferenceFields => _customerReferenceFields.AsReadOnly();

        public void AddCustomerReferenceField(CustomerReferenceField customerReferenceField, Guid callerId)
        {
            if (!_customerReferenceFields.Any(r => r.Name == customerReferenceField.Name && r.ReferenceType == customerReferenceField.ReferenceType))
            {
                _customerReferenceFields.Add(customerReferenceField);
                AddDomainEvent(new CustomerReferenceFieldAddedDomainEvent(CustomerId, customerReferenceField, callerId));
            }
        }

        public void AddCustomerOperatorSettings(List<CustomerOperatorSettings> customerOperatorSettings)
        {

            //Need to add domain event
            _customerOperatorSettings.AddRange(customerOperatorSettings);
        }

        public void RemoveCustomerOperatorSettings(int operatorId)
        {
            var customerOperatorSettings = _customerOperatorSettings.FirstOrDefault(x => x.Operator.Id == operatorId);

            if (customerOperatorSettings != null)
            {
                _customerOperatorSettings.Remove(customerOperatorSettings);
                AddDomainEvent(new CustomerOperatorSettingsRemovedDomainEvent(CustomerId, customerOperatorSettings, operatorId));
                return;
            }

            throw new ArgumentException($"Operator {operatorId} is not associated with this customer.");
        }

        public CustomerSubscriptionProduct AddSubscriptionProduct(Operator @operator, string productName, IList<string>? selectedDataPackages, SubscriptionProduct? globalSubscriptionProduct, Guid callerId)
        {
            //If the operator settings is null then add operator to customer
            var customerOperatorSetting = CustomerOperatorSettings.FirstOrDefault(os => os.Operator.Id == @operator.Id);
            if (customerOperatorSetting == null)
            {
                customerOperatorSetting = new CustomerOperatorSettings(@operator, null, callerId);
                AddDomainEvent(new CustomerOperatorSettingsAddedDomainEvent(CustomerId,customerOperatorSetting, callerId));
                _customerOperatorSettings.Add(customerOperatorSetting);
            }

            var foundSubscriptionProduct = customerOperatorSetting.AvailableSubscriptionProducts.FirstOrDefault(p => p.SubscriptionName == productName);
            if (foundSubscriptionProduct == null)
            {
               
               var dataPackages = selectedDataPackages?.Select(dataPackage => new DataPackage(dataPackage, callerId)).ToList();
                var globalDataPackages = new List<DataPackage>();
                if (globalSubscriptionProduct != null && globalSubscriptionProduct.DataPackages.Any() && selectedDataPackages != null) {
                    foreach (var dataPackage in selectedDataPackages)
                    {
                        var existingDataPackage = globalSubscriptionProduct.DataPackages.FirstOrDefault(a => a.DataPackageName == dataPackage);
                        if (existingDataPackage != null)
                        {
                            globalDataPackages.Add(existingDataPackage);
                        }
                    }
                }
                var customerSubscriptionProduct = globalSubscriptionProduct != null
                    ? new CustomerSubscriptionProduct(globalSubscriptionProduct, callerId, globalDataPackages)
                    : new CustomerSubscriptionProduct(productName, @operator, callerId, dataPackages);
                foundSubscriptionProduct = customerSubscriptionProduct;
                customerOperatorSetting.AvailableSubscriptionProducts.Add(customerSubscriptionProduct);

                if (globalSubscriptionProduct != null)
                {

                    //Global
                    AddDomainEvent(new CustomerSubscriptionProductAddedDomainEvent(CustomerId, customerSubscriptionProduct, callerId));
                }
                else
                {
                    //Custom
                    AddDomainEvent(new DatapackagesAddedDomainEvent(dataPackages,CustomerId, callerId));
                    AddDomainEvent(new CustomerSubscriptionProductAddedDomainEvent(CustomerId, customerSubscriptionProduct, callerId));
                }
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
            AddDomainEvent(new CustomerReferenceFieldRemovedDomainEvent(customerReferenceField,CustomerId));
            return customerReferenceField;
        }

        public CustomerSubscriptionProduct? RemoveSubscriptionProduct(int subscriptionProductId)
        {
            var subscriptionProduct = _customerOperatorSettings.SelectMany(a => a.AvailableSubscriptionProducts).FirstOrDefault(a => a.Id == subscriptionProductId);

            if (subscriptionProduct == null)
            {
                return null;
            }

            var customerOperatorSetting = _customerOperatorSettings.FirstOrDefault(r => r.Operator.Id == subscriptionProduct.Operator.Id);

            if (customerOperatorSetting == null)
            {
                return null;
            }

            customerOperatorSetting.AvailableSubscriptionProducts.Remove(subscriptionProduct);
            AddDomainEvent(new CustomerSubscriptionProductdRemovedDomainEvent(subscriptionProduct, CustomerId));

            return subscriptionProduct;
        }

        public CustomerOperatorAccount AddCustomerOperatorAccount(string accountNumber, string accountName, Operator @operator, Guid callerId)
        {
            var customerOperatorSettings = _customerOperatorSettings.FirstOrDefault(o => o.Operator.Id == @operator.Id);
            var customerOperatorAccount = new CustomerOperatorAccount(CustomerId, accountNumber, accountName, @operator.Id, callerId);
            if (customerOperatorSettings == null)
            {
                customerOperatorSettings = new CustomerOperatorSettings(@operator, new List<CustomerOperatorAccount> { customerOperatorAccount }, callerId);
            }
            else
            {
                if (CustomerOperatorSettings.FirstOrDefault(s => s.Operator.Id == @operator.Id)!.CustomerOperatorAccounts.Any(a => a.AccountNumber == accountNumber))
                {
                    throw new ArgumentException(
                        $"A customer operator account with organization ID ({CustomerId}) and account number {accountNumber} already exists.");
                }
            }

            customerOperatorAccount.CustomerOperatorSetting = customerOperatorSettings;
            customerOperatorSettings.CustomerOperatorAccounts.Add(customerOperatorAccount);
            return customerOperatorAccount;
        }

        public void DeleteCustomerOperatorAccountAsync(string accountNumber, Operator @operator)
        {
            var customerOperatorAccount = _customerOperatorSettings.FirstOrDefault(o => o.Operator.Id == @operator.Id)
                ?.CustomerOperatorAccounts
                .FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (customerOperatorAccount != null)
            {
                _customerOperatorSettings.FirstOrDefault(o => o.Operator.Id == @operator.Id)
                    ?.CustomerOperatorAccounts
                    .Remove(customerOperatorAccount);
            }
            else
            {
                throw new ArgumentException(
                    $"No customer operator account with organization ID({CustomerId}) and account name AC_NUM11 exists.");
            }
        }

        public CustomerSubscriptionProduct UpdateSubscriptionProduct(CustomerSubscriptionProductDTO customerSubscriptionProduct)
        {
            var foundProduct = CustomerOperatorSettings.SelectMany(os => os.AvailableSubscriptionProducts)?.FirstOrDefault();
            if (foundProduct == null)
            {
                throw new ArgumentException("A customer subscription product not found. Unable to update.");
            }

            foundProduct.SubscriptionName = customerSubscriptionProduct.Name;
            // Check for any new data packages.
            foreach (var datapackage in customerSubscriptionProduct.Datapackages)
            {
                if (foundProduct.DataPackages.All(dp => dp.DataPackageName != datapackage))
                {
                    foundProduct.DataPackages.Add(new DataPackage(datapackage, Guid.Empty));
                }
            }
            // Check if any data packages have been deleted.
            foreach (var datapackage in foundProduct.DataPackages)
            {
                if (customerSubscriptionProduct.Datapackages.All(dataPackageName => dataPackageName != datapackage.DataPackageName))
                {
                    foundProduct.DataPackages.Remove(datapackage);
                }
            }

            return foundProduct;
        }
    }
}
