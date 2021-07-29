using Common.Enums;
using CustomerServices.DomainEvents;
using CustomerServices.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerServices
{
    public class CustomerServices : ICustomerServices
    {
        private readonly ILogger<CustomerServices> _logger;
        private readonly ICustomerRepository _customerRepository;

        public CustomerServices(ILogger<CustomerServices> logger, ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        public async Task<IList<Customer>> GetCustomersAsync()
        {
            return await _customerRepository.GetCustomersAsync();
        }


        public async Task<Customer> AddCustomerAsync(string companyName, string orgNumber, string contactPersonFullName, string contactPersonEmail,
            string contactPersonPhoneNumber, string companyAddressStreet, string companyAddressPostCode,
            string companyAddressCity, string companyAddressCountry)
        {
            var companyAddress = new Address(companyAddressStreet, companyAddressPostCode, companyAddressCity, companyAddressCountry);
            var contactPerson = new ContactPerson(contactPersonFullName, contactPersonEmail, contactPersonPhoneNumber);
            var newCustomer = new Customer(Guid.NewGuid(), companyName, orgNumber, companyAddress, contactPerson);
            return await _customerRepository.AddAsync(newCustomer);
        }

        public async Task<Customer> GetCustomerAsync(Guid customerId)
        {
            return await _customerRepository.GetCustomerAsync(customerId);
        }

        public async Task<IList<AssetCategoryLifecycleType>> GetAssetCategoryLifecycleType(Guid customerId, Guid assetCategoryId)
        {
            return await _customerRepository.GetAssetCategoryLifecycleType(customerId, assetCategoryId);
        }

        public async Task<IList<AssetCategoryLifecycleType>> GetAllAssetCategoryLifecycleTypesForCustomerAsync(Guid customerId)
        {
            return await _customerRepository.GetAllAssetCategoryLifecycleTypesAsync(customerId);
        }

        public async Task<AssetCategoryLifecycleType> AddAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, Guid assetCategoryId, int lifecycle)
        {
            var customer = await GetCustomerAsync(customerId);
            var assetCategory = await GetAssetCategoryType(customerId, assetCategoryId);
            var assetCategoryLifecycles = await GetAllAssetCategoryLifecycleTypesForCustomerAsync(customerId);
            var assetLifecycle = assetCategoryLifecycles.FirstOrDefault(l => l.CustomerId == customerId && l.AssetCategoryId == assetCategoryId && l.LifecycleType == (LifecycleType)lifecycle);
            if (customer == null || assetCategory == null)
            {
                return null;
            }
            if (assetLifecycle != null)
                return assetLifecycle;
            var assetCategoryLifecycle = new AssetCategoryLifecycleType(customerId, assetCategoryId, lifecycle);
            assetCategory.LifecycleTypes.Add(assetCategoryLifecycle);
            customer.SelectedAssetCategoryLifecycles.Add(assetCategoryLifecycle);
            await _customerRepository.SaveChanges();
            return assetCategoryLifecycle;
        }

        public async Task<AssetCategoryLifecycleType> RemoveAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, Guid assetCategoryId, int lifecycle)
        {
            var customer = await GetCustomerAsync(customerId);
            var assetCategoryLifecycle = await GetAssetCategoryLifecycleType(customerId, assetCategoryId);
            if (customer == null)
            {
                return null;
            }
            var deleteAssetLifecycle = assetCategoryLifecycle.FirstOrDefault(l => (int)l.LifecycleType == lifecycle);

            return await _customerRepository.DeleteAssetCategoryLifecycleTypeAsync(deleteAssetLifecycle);
        }

        public async Task<AssetCategoryType> GetAssetCategoryType(Guid customerId, Guid assetCategoryId)
        {
            return await _customerRepository.GetAssetCategoryTypeAsync(customerId, assetCategoryId);
        }

        public async Task<IList<AssetCategoryType>> GetAssetCategoryTypes(Guid customerId)
        {
            var customerCategories = await _customerRepository.GetAssetCategoryTypesAsync(customerId);
            var categoryLifecycles = await GetAllAssetCategoryLifecycleTypesForCustomerAsync(customerId);
            foreach (var category in customerCategories)
            {
                category.LifecycleTypes = categoryLifecycles.Where(l => category.LifecycleTypes.Contains(l)).ToList();
            }

            return customerCategories;
        }

        public async Task<AssetCategoryType> AddAssetCategoryType(Guid customerId, Guid assetCategoryId)
        {
            var customer = await GetCustomerAsync(customerId);
            var assetCategory = await GetAssetCategoryType(customerId, assetCategoryId);
            if (customer == null)
            {
                return null;
            }
            // If it exist don't create a new one :)
            if (assetCategory != null)
                return assetCategory;
            assetCategory = new AssetCategoryType
            {
                ExternalCustomerId = customerId,
                AssetCategoryId = assetCategoryId,
                LifecycleTypes = new List<AssetCategoryLifecycleType>()
            };
            customer.SelectedAssetCategories.Add(assetCategory);
            await _customerRepository.SaveChanges();
            return assetCategory;
        }

        public async Task<AssetCategoryType> RemoveAssetCategoryType(Guid customerId, Guid assetCategoryId)
        {
            var customer = await GetCustomerAsync(customerId);
            var assetCategory = await GetAssetCategoryType(customerId, assetCategoryId);
            if (customer == null || assetCategory == null)
            {
                return null;
            }
            var activeLifecycles = await GetAllAssetCategoryLifecycleTypesForCustomerAsync(customerId);
            // Also remove lifecycles for this category when this category is removed  
            var lifecycles = activeLifecycles.Where(l => l.AssetCategoryId == assetCategory.AssetCategoryId);
            foreach (var lifecycle in lifecycles)
            {
                await RemoveAssetCategoryLifecycleTypeForCustomerAsync(customerId, lifecycle.AssetCategoryId, (int)lifecycle.LifecycleType);
            }
            return await _customerRepository.DeleteAssetCategoryTypeAsync(assetCategory);
        }

        public async Task<ProductModuleGroup> GetProductModuleGroup(Guid moduleGroupId)
        {
            return await _customerRepository.GetProductModuleGroupAsync(moduleGroupId);
        }

        public async Task<IList<ProductModuleGroup>> GetCustomerProductModuleGroupsAsync(Guid customerId)
        {
            return await _customerRepository.GetCustomerProductModuleGroupsAsync(customerId);
        }

        public async Task<ProductModuleGroup> AddProductModuleGroupsAsync(Guid customerId, Guid moduleGroupId)
        {
            var customer = await GetCustomerAsync(customerId);
            var moduleGroup = await GetProductModuleGroup(moduleGroupId);
            if (customer == null)
            {
                return null;
            }
            customer.SelectedProductModuleGroups.Add(moduleGroup);
            await _customerRepository.SaveChanges();
            return moduleGroup;
        }

        public async Task<ProductModuleGroup> RemoveProductModuleGroupsAsync(Guid customerId, Guid moduleGroupId)
        {
            var customer = await GetCustomerAsync(customerId);
            var moduleGroup = await GetProductModuleGroup(moduleGroupId);
            if (customer == null)
            {
                return null;
            }
            customer.SelectedProductModuleGroups.Remove(moduleGroup);
            await _customerRepository.SaveChanges();
            return moduleGroup;
        }

        public async Task<ProductModule> GetProductModule(Guid moduleId)
        {
            return await _customerRepository.GetProductModuleAsync(moduleId);
        }

        public async Task<IList<ProductModule>> GetCustomerProductModulesAsync(Guid customerId)
        {
            return await _customerRepository.GetCustomerProductModulesAsync(customerId);
        }

        public async Task<ProductModule> AddProductModulesAsync(Guid customerId, Guid moduleId)
        {
            var customer = await GetCustomerAsync(customerId);
            var moduleGroup = await GetProductModule(moduleId);
            if (customer == null)
            {
                return null;
            }
            customer.SelectedProductModules.Add(moduleGroup);
            await _customerRepository.SaveChanges();
            return moduleGroup;
        }

        public async Task<ProductModule> RemoveProductModulesAsync(Guid customerId, Guid moduleId)
        {
            var customer = await GetCustomerAsync(customerId);
            var module = await GetProductModule(moduleId);
            foreach (var moduleGroup in module.ProductModuleGroup)
            {
                await RemoveProductModuleGroupsAsync(customerId, moduleGroup.ProductModuleGroupId);
            }
            if (customer == null)
            {
                return null;
            }
            customer.SelectedProductModules.Remove(module);
            await _customerRepository.SaveChanges();
            return module;
        }
    }
}