using Common.Seedwork;
using SubscriptionManagementServices.DomainEvents;
using System.Text.Json.Serialization;
using SubscriptionManagementServices.ServiceModels;
using SubscriptionManagementServices.Exceptions;

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

        public CustomerSettings() { }

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
                AddDomainEvent(new CustomerOperatorSettingsAddedDomainEvent(CustomerId, customerOperatorSetting, callerId));
                _customerOperatorSettings.Add(customerOperatorSetting);
            }

            var foundSubscriptionProduct = customerOperatorSetting.AvailableSubscriptionProducts.FirstOrDefault(p => p.SubscriptionName == productName);
            if (foundSubscriptionProduct == null)
            {

                var dataPackages = selectedDataPackages?.Select(dataPackage => new DataPackage(dataPackage, callerId)).ToList();
                var globalDataPackages = new List<DataPackage>();
                if (globalSubscriptionProduct != null && globalSubscriptionProduct.DataPackages.Any() && selectedDataPackages != null)
                {
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
                    AddDomainEvent(new DatapackagesAddedDomainEvent(dataPackages, CustomerId, callerId));
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
            AddDomainEvent(new CustomerReferenceFieldRemovedDomainEvent(customerReferenceField, CustomerId));
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
            AddDomainEvent(new CustomerSubscriptionProductRemovedDomainEvent(subscriptionProduct, CustomerId));

            return subscriptionProduct;
        }

        public CustomerOperatorAccount AddCustomerOperatorAccount(string accountNumber, string accountName, Operator @operator, Guid callerId, string? connectedOrganizationNumber)
        {
            var customerOperatorSettings = _customerOperatorSettings.FirstOrDefault(o => o.Operator.Id == @operator.Id);
            var customerOperatorAccount = new CustomerOperatorAccount(CustomerId, connectedOrganizationNumber, accountNumber, accountName, @operator.Id, callerId);
            if (customerOperatorSettings == null)
            {
                customerOperatorSettings = new CustomerOperatorSettings(@operator, new List<CustomerOperatorAccount> { customerOperatorAccount }, callerId);
                _customerOperatorSettings.Add(customerOperatorSettings);
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
            var foundProduct = CustomerOperatorSettings.SelectMany(os => os.AvailableSubscriptionProducts)?.FirstOrDefault(m => m.Id == customerSubscriptionProduct.Id);
            if (foundProduct == null)
            {
                throw new ArgumentException("A customer subscription product not found. Unable to update.");
            }


            if (foundProduct.GlobalSubscriptionProduct != null)
                foundProduct.SubscriptionName = customerSubscriptionProduct.Name;

            foundProduct.SetDataPackages(customerSubscriptionProduct.DataPackages);

            return foundProduct;
        }

        public CustomerStandardPrivateSubscriptionProduct AddCustomerStandardPrivateSubscriptionProduct(NewCustomerStandardPrivateSubscriptionProduct standardProduct)
        {

            var customerOperatorSetting = CustomerOperatorSettings.FirstOrDefault(a => a.Operator.Id == standardProduct.OperatorId);
            if (customerOperatorSetting == null) throw new CustomerSettingsException($"Customer don't have operator with id {standardProduct.OperatorId} as a setting",Guid.Parse("e0903419-480c-4ea2-926b-14bf84f3819b"));

            var customerStandardPrivateSubscriptionProduct = new CustomerStandardPrivateSubscriptionProduct(standardProduct.DataPackage,standardProduct.SubscriptionName, standardProduct.CallerId);

            customerOperatorSetting.StandardPrivateSubscriptionProduct = customerStandardPrivateSubscriptionProduct;
            AddDomainEvent(new CustomerStandardPrivateSubscriptionProductAddedDomainEvent(CustomerId, customerOperatorSetting.Operator.OperatorName, customerStandardPrivateSubscriptionProduct, standardProduct.CallerId));

            return customerStandardPrivateSubscriptionProduct;
        }
        public CustomerStandardPrivateSubscriptionProduct RemoveCustomerStandardPrivateSubscriptionProduct(int operatorId, Guid callerId)
        {
            var customerOperatorSetting = CustomerOperatorSettings.FirstOrDefault(a => a.Operator.Id == operatorId && a.StandardPrivateSubscriptionProduct != null);
            if (customerOperatorSetting == null) throw new CustomerSettingsException($"Customer don't have standard private product set up for {operatorId}", Guid.Parse("1242b8f2-27dd-4ca6-b91c-ac1de548bc11"));

            var standardProduct = customerOperatorSetting.StandardPrivateSubscriptionProduct;
            customerOperatorSetting.StandardPrivateSubscriptionProduct = null;
            AddDomainEvent(new CustomerStandardPrivateSubscriptionProductRemovedDomainEvent(standardProduct, operatorId, callerId, CustomerId));
            return standardProduct;
        }
        
        public CustomerStandardBusinessSubscriptionProduct AddCustomerStandardBusinessSubscriptionProduct(NewCustomerStandardBusinessSubscriptionProduct businessProduct)
        {

            var customerOperatorSetting = CustomerOperatorSettings.FirstOrDefault(a => a.Operator.Id == businessProduct.OperatorId);
            if (customerOperatorSetting == null) throw new CustomerSettingsException($"Customer don't have operator with id {businessProduct.OperatorId} as a setting", Guid.Parse("5ea10a47-589a-4047-aa99-151db883f824"));

            var customerStandardBusinessSubscriptionProduct = 
                new CustomerStandardBusinessSubscriptionProduct(
                businessProduct.DataPackage,
                businessProduct.SubscriptionName,
                businessProduct.CallerId,
                businessProduct.AddOnProducts.Select(a => new SubscriptionAddOnProduct(a, businessProduct.CallerId)).ToList()
                );

            customerOperatorSetting.StandardBusinessSubscriptionProduct = customerStandardBusinessSubscriptionProduct;
            AddDomainEvent(new CustomerStandardBusinessSubscriptionProductAddedDomainEvent(CustomerId, customerOperatorSetting.Operator.OperatorName, customerStandardBusinessSubscriptionProduct, businessProduct.CallerId));

            return customerStandardBusinessSubscriptionProduct;
        }
        public CustomerStandardBusinessSubscriptionProduct RemoveCustomerStandardBusinessSubscriptionProduct(int operatorId, Guid callerId)
        {
            var customerOperatorSetting = CustomerOperatorSettings.FirstOrDefault(a => a.Operator.Id == operatorId && a.StandardBusinessSubscriptionProduct != null);
            if (customerOperatorSetting == null) throw new CustomerSettingsException($"Customer don't have standard business product set up for {operatorId}", Guid.Parse("fae81e86-cc80-4b3e-87e6-83c1c0827e94"));

            var standardProduct = customerOperatorSetting.StandardBusinessSubscriptionProduct;
            customerOperatorSetting.StandardBusinessSubscriptionProduct = null;
            AddDomainEvent(new CustomerStandardBusinessSubscriptionProductRemovedDomainEvent(standardProduct, operatorId, callerId, CustomerId));
            return standardProduct;
        }
    }
}
