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

        public async Task<IList<AssetCategoryLifecycleType>> RemoveAssetCategoryLifecycleTypesForCustomerAsync(IList<AssetCategoryLifecycleType> assetCategoryLifecycleTypes)
        {
            return await _customerRepository.DeleteAssetCategoryLifecycleTypeAsync(assetCategoryLifecycleTypes);
        }

        public async Task<AssetCategoryType> GetAssetCategoryType(Guid customerId, Guid assetCategoryId)
        {
            return await _customerRepository.GetAssetCategoryTypeAsync(customerId, assetCategoryId);
        }

        public async Task<IList<AssetCategoryType>> GetAssetCategoryTypes(Guid customerId)
        {
            var customerCategories = await _customerRepository.GetAssetCategoryTypesAsync(customerId);
            return customerCategories;
        }

        public async Task<AssetCategoryType> AddAssetCategoryType(Guid customerId, AssetCategoryType addedAssetCategory)
        {
            var customer = await GetCustomerAsync(customerId);
            var assetCategory = await GetAssetCategoryType(customerId, addedAssetCategory.AssetCategoryId);
            if (customer == null)
            {
                return null;
            }
            if (assetCategory != null)
            {
                assetCategory.SetAssetCategoryId(addedAssetCategory.AssetCategoryId);
                foreach (var lifecycle in addedAssetCategory.LifecycleTypes)
                {
                    var exist = assetCategory.LifecycleTypes.FirstOrDefault(a => a.LifecycleType == lifecycle.LifecycleType);
                    if (exist == null)
                        assetCategory.LifecycleTypes.Add(lifecycle);
                }
            }
            else
            {
                customer.SelectedAssetCategories.Add(addedAssetCategory);
            }
            await _customerRepository.SaveChanges();
            // return updated object
            return await GetAssetCategoryType(customerId, addedAssetCategory.AssetCategoryId);
        }

        public async Task<AssetCategoryType> RemoveAssetCategoryType(Guid customerId, AssetCategoryType deletedAssetCategory)
        {
            var customer = await GetCustomerAsync(customerId);
            var assetCategory = await GetAssetCategoryType(customerId, deletedAssetCategory.AssetCategoryId);
            if (customer == null || assetCategory == null)
            {
                return null;
            }
            // If no lifecycles are selected delete the asset category as well
            if (!deletedAssetCategory.LifecycleTypes.Any())
            {
                await RemoveAssetCategoryLifecycleTypesForCustomerAsync(assetCategory.LifecycleTypes);
                return await _customerRepository.DeleteAssetCategoryTypeAsync(assetCategory);
            }
            var lifecycles = deletedAssetCategory.LifecycleTypes.Select(a => a.LifecycleType);
            var deleteList = assetCategory.LifecycleTypes.Where(a => lifecycles.Contains(a.LifecycleType)).ToList();
            // Delete lifecycles of this asset category
            await RemoveAssetCategoryLifecycleTypesForCustomerAsync(deleteList);
            // return updated object
            return await GetAssetCategoryType(customerId, deletedAssetCategory.AssetCategoryId);
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