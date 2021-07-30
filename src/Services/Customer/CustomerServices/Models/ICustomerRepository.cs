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

        Task<IList<AssetCategoryLifecycleType>> DeleteAssetCategoryLifecycleTypeAsync(IList<AssetCategoryLifecycleType> assetCategoryLifecycleTypes);
        Task<IList<AssetCategoryType>> GetAssetCategoryTypesAsync(Guid customerId);
        Task<AssetCategoryType> GetAssetCategoryTypeAsync(Guid customerId, Guid assetCategoryId);
        Task<AssetCategoryType> DeleteAssetCategoryTypeAsync(AssetCategoryType assetCategoryType);

        Task<IList<ProductModuleGroup>> GetProductModuleGroupsAsync();
        Task<IList<ProductModuleGroup>> GetCustomerProductModuleGroupsAsync(Guid customerId);
        Task<ProductModuleGroup> GetProductModuleGroupAsync(Guid moduleGroupId);

        Task<IList<ProductModule>> GetProductModulesAsync();
        Task<IList<ProductModule>> GetCustomerProductModulesAsync(Guid customerId);
        Task<ProductModule> GetProductModuleAsync(Guid moduleId);
    }
}
