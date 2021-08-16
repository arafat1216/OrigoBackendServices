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

        Task<IList<AssetCategoryLifecycleType>> RemoveAssetCategoryLifecycleTypesForCustomerAsync(Customer customer, AssetCategoryType assetCategory, IList<AssetCategoryLifecycleType> assetCategoryLifecycleTypes);
        Task<AssetCategoryType> GetAssetCategoryType(Guid customerId, Guid assetCategoryId);
        Task<IList<AssetCategoryType>> GetAssetCategoryTypes(Guid customerId);
        Task<AssetCategoryType> AddAssetCategoryType(Guid customerId, Guid addedAssetCategoryId, IList<int> lifecycleTypes);
        Task<AssetCategoryType> RemoveAssetCategoryType(Guid customerId, Guid deletedAssetCategoryId, IList<int> lifecycleTypes);

        Task<IList<ProductModule>> GetCustomerProductModulesAsync(Guid customerId);
        Task<ProductModule> AddProductModulesAsync(Guid customerId, Guid moduleId, IList<Guid> productModuleGroupIds);
        Task<ProductModule> RemoveProductModulesAsync(Guid customerId, Guid moduleId, IList<Guid> productModuleGroupIds);

        Task<Customer> AddCustomerAsync(string companyName, string orgNumber, string contactPersonFullName, string contactPersonEmail, string contactPersonPhoneNumber, string companyAddressStreet, string companyAddressPostCode, string companyAddressCity, string companyAddressCountry);
    }
}