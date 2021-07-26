﻿using OrigoApiGateway.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface ICustomerServices
    {
        Task<IList<OrigoCustomer>> GetCustomersAsync();
        Task<OrigoCustomer> GetCustomerAsync(Guid customerId);
        Task<OrigoCustomer> CreateCustomerAsync(OrigoNewCustomer newCustomer);
        Task<IList<OrigoAssetCategoryLifecycleType>> GetAssetCategoryLifecycleTypesForCustomerAsync(Guid customerId);
        Task<OrigoAssetCategoryLifecycleType> AddAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, Guid assetCategoryId, int lifecycle);
        Task<OrigoAssetCategoryLifecycleType> RemoveAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, Guid assetCategoryId, int lifecycle);
        Task<IList<OrigoCustomerAssetCategoryType>> GetAssetCategoryForCustomerAsync(Guid customerId);
        Task<OrigoCustomerAssetCategoryType> AddAssetCategoryForCustomerAsync(Guid customerId, Guid assetCategoryId);
        Task<OrigoCustomerAssetCategoryType> RemoveAssetCategoryForCustomerAsync(Guid customerId, Guid assetCategoryId);
        Task<IList<OrigoProductModuleGroup>> GetCustomerProductModulesAsync(Guid customerId);
        Task<OrigoProductModuleGroup> AddProductModulesAsync(Guid customerId, Guid moduleGroupId);
        Task<OrigoProductModuleGroup> RemoveProductModulesAsync(Guid customerId, Guid moduleGroupId);
    }
}