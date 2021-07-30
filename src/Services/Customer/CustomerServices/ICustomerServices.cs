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

        Task<IList<AssetCategoryLifecycleType>> RemoveAssetCategoryLifecycleTypesForCustomerAsync(IList<AssetCategoryLifecycleType> assetCategoryLifecycleTypes);
        Task<AssetCategoryType> GetAssetCategoryType(Guid customerId, Guid assetCategoryId);
        Task<IList<AssetCategoryType>> GetAssetCategoryTypes(Guid customerId);
        Task<AssetCategoryType> AddAssetCategoryType(Guid customerId, AssetCategoryType addedAssetCategory);
        Task<AssetCategoryType> RemoveAssetCategoryType(Guid customerId, AssetCategoryType deletedAssetCategory);

        Task<ProductModuleGroup> GetProductModuleGroup(Guid moduleGroupId);
        Task<IList<ProductModuleGroup>> GetCustomerProductModuleGroupsAsync(Guid customerId);
        Task<ProductModuleGroup> AddProductModuleGroupsAsync(Guid customerId, Guid moduleGroupId);
        Task<ProductModuleGroup> RemoveProductModuleGroupsAsync(Guid customerId, Guid moduleGroupId);

        Task<ProductModule> GetProductModule(Guid moduleId);
        Task<IList<ProductModule>> GetCustomerProductModulesAsync(Guid customerId);
        Task<ProductModule> AddProductModulesAsync(Guid customerId, Guid moduleId);
        Task<ProductModule> RemoveProductModulesAsync(Guid customerId, Guid moduleId);
    }
}