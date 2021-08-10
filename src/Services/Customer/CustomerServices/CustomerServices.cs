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
            await _customerRepository.SaveEntitiesAsync();
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

        public async Task<IList<ProductModule>> GetCustomerProductModulesAsync(Guid customerId)
        {
            return await _customerRepository.GetCustomerProductModulesAsync(customerId);
        }

        public async Task<ProductModule> AddProductModulesAsync(Guid customerId, Guid moduleId, IList<Guid> productModuleGroupIds)
        {
            var productModule = await _customerRepository.AddProductModuleAsync(customerId, moduleId);
            if (productModule != null)
            {
                foreach (var moduleGroupId in productModuleGroupIds)
                {
                    await _customerRepository.AddProductModuleGroupAsync(customerId, moduleGroupId);
                }
            }
            return GetCustomerProductModulesAsync(customerId).Result.FirstOrDefault(m => m.ProductModuleId == moduleId);
        }

        public async Task<ProductModule> RemoveProductModulesAsync(Guid customerId, Guid moduleId, IList<Guid> productModuleGroupIds)
        {
            var customerModules = await GetCustomerProductModulesAsync(customerId);
            var module = customerModules.FirstOrDefault(m => m.ProductModuleId == moduleId);
            if (module == null)
            {
                return null;
            }
            if (!productModuleGroupIds.Any()) // remove module and module groups
            {
                foreach (var moduleGroup in module.ProductModuleGroup)
                {
                    await _customerRepository.RemoveProductModuleGroupAsync(customerId, moduleGroup.ProductModuleGroupId);
                }
                await _customerRepository.RemoveProductModuleAsync(customerId, moduleId);
                return GetCustomerProductModulesAsync(customerId).Result.FirstOrDefault(m => m.ProductModuleId == moduleId);
            }
            foreach (var groupId in productModuleGroupIds) // remove module groups
            {
                await _customerRepository.RemoveProductModuleGroupAsync(customerId, groupId);
            }
            return GetCustomerProductModulesAsync(customerId).Result.FirstOrDefault(m => m.ProductModuleId == moduleId);
        }
    }
}