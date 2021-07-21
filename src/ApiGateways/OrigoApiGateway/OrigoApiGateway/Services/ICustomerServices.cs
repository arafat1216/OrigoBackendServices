using OrigoApiGateway.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrigoApiGateway.Controllers;

namespace OrigoApiGateway.Services
{
    public interface ICustomerServices
    {
        Task<IList<OrigoCustomer>> GetCustomersAsync();
        Task<OrigoCustomer> GetCustomerAsync(Guid customerId);
        Task<OrigoCustomer> CreateCustomerAsync(OrigoNewCustomer newCustomer);
        Task<IList<OrigoAssetCategoryLifecycleType>> GetAssetCategoryLifecycleTypesForCustomerAsync(Guid customerId);
        Task<OrigoAssetCategoryLifecycleType> AddAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, Guid assetCategoryLifecycleId);
        Task<OrigoAssetCategoryLifecycleType> RemoveAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, Guid assetCategoryLifecycleId);
        Task<IList<OrigoAssetCategoryType>> GetAssetCategoryForCustomerAsync(Guid customerId);
        Task<OrigoAssetCategoryType> AddAssetCategoryForCustomerAsync(Guid customerId, Guid assetCategoryId);
        Task<OrigoAssetCategoryType> RemoveAssetCategoryForCustomerAsync(Guid customerId, Guid assetCategoryId);
        Task<IList<OrigoProductModuleGroup>> GetCustomerProductModulesAsync(Guid customerId);
        Task<OrigoProductModuleGroup> AddProductModulesAsync(Guid customerId, Guid moduleGroupId);
        Task<OrigoProductModuleGroup> RemoveProductModulesAsync(Guid customerId, Guid moduleGroupId);
    }
}