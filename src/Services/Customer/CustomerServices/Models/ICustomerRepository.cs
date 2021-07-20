using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices.Models
{
    public interface ICustomerRepository
    {
        Task SaveChanges();
        Task<Customer> AddAsync(Customer customer);
        Task<IList<Customer>> GetCustomersAsync();
        Task<Customer> GetCustomerAsync(Guid customerId);

        Task<IList<User>> GetAllUsersAsync(Guid customerId);
        Task<User> GetUserAsync(Guid customerId, Guid userId);
        Task<User> AddUserAsync(User newUser);

        Task<IList<AssetCategoryLifecycleType>> GetAllAssetCategoryLifecycleTypesAsync(Guid assetCategoryId);
        Task<AssetCategoryLifecycleType> GetAssetCategoryLifecycleType(Guid customerId, Guid assetCategoryId);
        Task<AssetCategoryLifecycleType> AddAssetCategoryLifecycleTypeAsync(AssetCategoryLifecycleType newAssetCategoryLifecycleType);
        Task RemoveAssetCategoryLifecycleType(AssetCategoryLifecycleType deleteAssetCategoryLifecycleType);

        Task<IList<AssetCategoryType>> GetAssetCategoriesAsync();
        Task<IList<AssetCategoryType>> GetAssetCategoryTypesAsync(Guid customerId);
        Task<AssetCategoryType> GetAssetCategoryTypeAsync(Guid assetCategoryId);

        Task<IList<ProductModule>> GetModulesAsync();
        Task<IList<ProductModuleGroup>> GetCustomerProductModulesAsync(Guid customerId);
        Task<ProductModuleGroup> GetProductModuleGroupAsync(Guid moduleGroupId);
    }
}
