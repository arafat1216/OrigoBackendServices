﻿using Common.Cryptography;
using Common.Enums;
using CustomerServices.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CustomerServices
{
    public class CustomerServices : ICustomerServices
    {
        private readonly ILogger<CustomerServices> _logger;
        private readonly IOrganizationRepository _customerRepository;

        public CustomerServices(ILogger<CustomerServices> logger, IOrganizationRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        public async Task<IList<Organization>> GetCustomersAsync()
        {
            return await _customerRepository.GetOrganizationsAsync();
        }


        public async Task<Organization> AddCustomerAsync(string companyName, string orgNumber, string contactPersonFullName, string contactPersonEmail,
            string contactPersonPhoneNumber, string companyAddressStreet, string companyAddressPostCode,
            string companyAddressCity, string companyAddressCountry)
        {
            var companyAddress = new Address(companyAddressStreet, companyAddressPostCode, companyAddressCity, companyAddressCountry);
            var contactPerson = new ContactPerson(contactPersonFullName, contactPersonEmail, contactPersonPhoneNumber);
            var newCustomer = new Organization(Guid.NewGuid(), companyName, orgNumber, companyAddress, contactPerson);
            return await _customerRepository.AddAsync(newCustomer);
        }

        public async Task<Organization> GetCustomerAsync(Guid customerId)
        {
            return await _customerRepository.GetOrganizationAsync(customerId);
        }

        public async Task<IList<AssetCategoryLifecycleType>> RemoveAssetCategoryLifecycleTypesForCustomerAsync(Organization customer, AssetCategoryType assetCategory, IList<AssetCategoryLifecycleType> assetCategoryLifecycleTypes)
        {
            return await _customerRepository.DeleteAssetCategoryLifecycleTypeAsync(customer, assetCategory, assetCategoryLifecycleTypes);
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

        public async Task<AssetCategoryType> AddAssetCategoryType(Guid customerId, Guid addedAssetCategoryId, IList<int> lifecycleTypes)
        {
            var customer = await GetCustomerAsync(customerId);
            var assetCategory = await GetAssetCategoryType(customerId, addedAssetCategoryId);
            if (customer == null)
            {
                return null;
            }
            if (assetCategory != null)
            {
                assetCategory.SetAssetCategoryId(addedAssetCategoryId);
                foreach (var lifecycle in lifecycleTypes)
                {
                    var exist = assetCategory.LifecycleTypes.FirstOrDefault(a => a.LifecycleType == (LifecycleType)lifecycle);
                    if (exist == null)
                        customer.AddLifecyle(assetCategory, new AssetCategoryLifecycleType(customerId, addedAssetCategoryId, lifecycle));
                }
            }
            else
            {
                assetCategory = new AssetCategoryType(addedAssetCategoryId, customerId, new List<AssetCategoryLifecycleType>());
                customer.AddAssetCategory(assetCategory);
                foreach (int lifecycle in lifecycleTypes)
                {
                    customer.AddLifecyle(assetCategory, new AssetCategoryLifecycleType(customerId, addedAssetCategoryId, lifecycle));
                }
            }
            await _customerRepository.SaveEntitiesAsync();
            // return updated object
            return await GetAssetCategoryType(customerId, addedAssetCategoryId);
        }

        public async Task<AssetCategoryType> RemoveAssetCategoryType(Guid customerId, Guid deletedAssetCategoryId, IList<int> lifecycleTypes)
        {
            var customer = await GetCustomerAsync(customerId);
            var assetCategory = await GetAssetCategoryType(customerId, deletedAssetCategoryId);
            if (customer == null || assetCategory == null)
            {
                return null;
            }
            // If no lifecycles are selected delete the asset category as well
            if (!lifecycleTypes.Any())
            {
                await RemoveAssetCategoryLifecycleTypesForCustomerAsync(customer, assetCategory, assetCategory.LifecycleTypes.ToList());
                customer.RemoveAssetCategory(assetCategory);
                return await _customerRepository.DeleteAssetCategoryTypeAsync(assetCategory);
            }
            var lifecycles = assetCategory.LifecycleTypes.Where(a => lifecycleTypes.Contains((int)a.LifecycleType)).Select(a => a).ToList();
            // Delete lifecycles of this asset category
            await RemoveAssetCategoryLifecycleTypesForCustomerAsync(customer, assetCategory, lifecycles);
            // return updated object
            return await GetAssetCategoryType(customerId, deletedAssetCategoryId);
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
            var result = await GetCustomerProductModulesAsync(customerId);
            return result.FirstOrDefault(m => m.ProductModuleId == moduleId);
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
            var result = await GetCustomerProductModulesAsync(customerId);
            return result.FirstOrDefault(m => m.ProductModuleId == moduleId);
        }

        public async Task<string> EncryptDataForCustomer(Guid customerId, string message, byte[] secretKey, byte[] iv)
        {
            try
            {
                var customer = await _customerRepository.GetOrganizationAsync(customerId);

                if (customer == null)
                    return null;

                string salt = customer.OrganizationId.ToString();


                var encryptedMessage = Encryption.EncryptData(message, salt, secretKey, iv);

                return encryptedMessage;
            }
            catch (CryptographicException ex)
            {
                _logger.LogError("EncryptDataForCustomer failed with CryptographicException error: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("EncryptDataForCustomer failed with unknown error: " + ex.Message);
                throw;
            }
        }

        public async Task<string> DecryptDataForCustomer(Guid customerId, string encryptedData, byte[] secretKey, byte[] iv)
        {
            try
            {
                var customer = await _customerRepository.GetOrganizationAsync(customerId);
                if (customer == null)
                    return null;

                string salt = customer.OrganizationId.ToString();
                var decryptedMessage = Encryption.DecryptData(encryptedData, salt, secretKey, iv);

                return decryptedMessage;
            }
            catch (CryptographicException ex)
            {
                _logger.LogError("DecryptDataForCustomer failed with CryptographicException error: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("DecryptDataForCustomer failed with unknown error: " + ex.Message);
                throw;
            }
        }
    }
}