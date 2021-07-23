using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface ICustomerServices
    {
        Task<IList<Customer>> GetCustomersAsync();
        Task<Customer> AddCustomerAsync(Customer newCustomer);
        Task<Customer> GetCustomerAsync(Guid customerId);

        Task<IList<AssetCategoryLifecycleType>> GetAllAssetCategoryLifecycleTypesForCustomerAsync(Guid customerId);
        Task<IList<AssetCategoryLifecycleType>> GetAssetCategoryLifecycleType(Guid customerId, Guid assetCategoryId);
        Task<AssetCategoryLifecycleType> AddAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, Guid assetCategoryId, int lifecycle);
        Task<AssetCategoryLifecycleType> RemoveAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, Guid assetCategoryId, int lifecycle);

        Task<AssetCategoryType> GetAssetCategoryType(Guid customerId, Guid assetCategoryId);
        Task<IList<AssetCategoryType>> GetAssetCategoryTypes(Guid customerId);
        Task<AssetCategoryType> AddAssetCategoryType(Guid customerId, Guid assetCategoryId);
        Task<AssetCategoryType> RemoveAssetCategoryType(Guid customerId, Guid assetCategoryId);

        Task<ProductModuleGroup> GetProductModuleGroup(Guid moduleGroupId);
        Task<IList<ProductModuleGroup>> GetCustomerProductModulesAsync(Guid customerId);
        Task<ProductModuleGroup> AddProductModulesAsync(Guid customerId, Guid moduleGroupId);
        Task<ProductModuleGroup> RemoveProductModulesAsync(Guid customerId, Guid moduleGroupId);
    }
}