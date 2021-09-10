using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerServices.Models
{
    public interface ICustomerRepository
    {
        Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default);
        Task<Customer> AddAsync(Customer customer);
        Task<IList<Customer>> GetCustomersAsync();
        Task<Customer> GetCustomerAsync(Guid customerId);

        Task<IList<User>> GetAllUsersAsync(Guid customerId);
        Task<User> GetUserAsync(Guid customerId, Guid userId);
        Task<User> AddUserAsync(User newUser);

        Task<IList<AssetCategoryLifecycleType>> DeleteAssetCategoryLifecycleTypeAsync(Customer customer, AssetCategoryType assetCategory, IList<AssetCategoryLifecycleType> assetCategoryLifecycleTypes);
        Task<IList<AssetCategoryType>> GetAssetCategoryTypesAsync(Guid customerId);
        Task<AssetCategoryType> GetAssetCategoryTypeAsync(Guid customerId, Guid assetCategoryId);
        Task<AssetCategoryType> DeleteAssetCategoryTypeAsync(AssetCategoryType assetCategoryType);

        Task<ProductModuleGroup> GetProductModuleGroupAsync(Guid moduleGroupId);
        Task<ProductModuleGroup> AddProductModuleGroupAsync(Guid customerId, Guid moduleGroupId);
        Task<ProductModuleGroup> RemoveProductModuleGroupAsync(Guid customerId, Guid moduleGroupId);
        Task<IList<ProductModule>> GetProductModulesAsync();
        Task<IList<ProductModule>> GetCustomerProductModulesAsync(Guid customerId);
        Task<ProductModule> GetProductModuleAsync(Guid moduleId);
        Task<ProductModule> AddProductModuleAsync(Guid customerId, Guid moduleId);
        Task<ProductModule> RemoveProductModuleAsync(Guid customerId, Guid moduleId);

        Task<IList<Department>> GetDepartmentsAsync(Guid customerId);
        Task<Department> GetDepartmentAsync(Guid customerId, Guid departmentId);
        Task<IList<Department>> DeleteDepartmentsAsync(IList<Department> department);
    }
}
