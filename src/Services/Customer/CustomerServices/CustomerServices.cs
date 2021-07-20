﻿using CustomerServices.DomainEvents;
using CustomerServices.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        public async Task<Customer> AddCustomerAsync(Customer newCustomer)
        {
            newCustomer.AddDomainEvent(new CustomerCreatedDomainEvent(newCustomer));
            return await _customerRepository.AddAsync(newCustomer);
        }

        public async Task<Customer> GetCustomerAsync(Guid customerId)
        {
            return await _customerRepository.GetCustomerAsync(customerId);
        }

        public async Task<IList<AssetCategoryLifecycleType>> GetAllAssetCategoryLifecycleTypesForCustomerAsync(Guid customerId)
        {
            return await _customerRepository.GetAllAssetCategoryLifecycleTypesAsync(customerId);
        }

        public async Task<AssetCategoryLifecycleType> AddAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, Guid assetCategoryId, string lifecycleType)
        {
            var customer = await _customerRepository.GetCustomerAsync(customerId);
            if (customer == null)
            {
                return null;
            }

            var newAssetCategoryLifecycleType = new AssetCategoryLifecycleType(assetCategoryId, lifecycleType);

            return await _customerRepository.AddAssetCategoryLifecycleTypeAsync(newAssetCategoryLifecycleType);
        }

        public async Task<AssetCategoryLifecycleType> RemoveAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, Guid assetCategoryId, string lifecycleType)
        {
            var assetCategoryLifecycleType = await _customerRepository.GetAssetCategoryLifecycleType(customerId, assetCategoryId);
            if (assetCategoryLifecycleType == null)
            {
                return null;
            }

            await _customerRepository.RemoveAssetCategoryLifecycleType(assetCategoryLifecycleType);
            return assetCategoryLifecycleType;
        }

        public async Task<ProductModuleGroup> GetAssetCategoryType(Guid moduleGroupId)
        {
            return await _customerRepository.GetProductModuleGroupAsync(moduleGroupId);
        }

        public async Task<IList<ProductModuleGroup>> GetAssetCategoryTypes(Guid customerId)
        {
            return await _customerRepository.GetCustomerProductModulesAsync(customerId);
        }

        public async Task<ProductModuleGroup> AddAssetCategoryType(Guid customerId, Guid assetCategoryId)
        {
            var customer = await GetCustomerAsync(customerId);
            var moduleGroup = await GetProductModuleGroup(assetCategoryId);
            if (customer == null)
            {
                return null;
            }
            customer.SelectedProductModuleGroups.Add(moduleGroup);
            await _customerRepository.SaveChanges();
            return moduleGroup;
        }

        public async Task<ProductModuleGroup> RemoveAssetCategoryType(Guid customerId, Guid AssetCategoryId)
        {
            var customer = await GetCustomerAsync(customerId);
            var moduleGroup = await GetProductModuleGroup(AssetCategoryId);
            if (customer == null)
            {
                return null;
            }
            customer.SelectedProductModuleGroups.Remove(moduleGroup);
            await _customerRepository.SaveChanges();
            return moduleGroup;
        }

        public async Task<ProductModuleGroup> GetProductModuleGroup(Guid moduleGroupId)
        {
            return await _customerRepository.GetProductModuleGroupAsync(moduleGroupId);
        }

        public async Task<IList<ProductModuleGroup>> GetCustomerProductModulesAsync(Guid customerId) 
        {
            return await _customerRepository.GetCustomerProductModulesAsync(customerId);
        }
        
        public async Task<ProductModuleGroup> AddProductModulesAsync(Guid customerId, Guid moduleGroupId)
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

        public async Task<ProductModuleGroup> RemoveProductModulesAsync(Guid customerId, Guid moduleGroupId)
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
    }
}