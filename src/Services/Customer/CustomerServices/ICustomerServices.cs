using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface ICustomerServices
    {
        Task<IList<Customer>> GetCustomersAsync();
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
        Task<IList<ProductModuleGroup>> GetCustomerProductModuleGroupsAsync(Guid customerId);
        Task<ProductModuleGroup> AddProductModuleGroupsAsync(Guid customerId, Guid moduleGroupId);
        Task<ProductModuleGroup> RemoveProductModuleGroupsAsync(Guid customerId, Guid moduleGroupId);

        Task<ProductModule> GetProductModule(Guid moduleId);
        Task<IList<ProductModule>> GetCustomerProductModulesAsync(Guid customerId);
        Task<ProductModule> AddProductModulesAsync(Guid customerId, Guid moduleId);
        Task<ProductModule> RemoveProductModulesAsync(Guid customerId, Guid moduleId);
        Task<Customer> AddCustomerAsync(string companyName, string orgNumber, string contactPersonFullName, string contactPersonEmail, string contactPersonPhoneNumber, string companyAddressStreet, string companyAddressPostCode, string companyAddressCity, string companyAddressCountry);
    }
}