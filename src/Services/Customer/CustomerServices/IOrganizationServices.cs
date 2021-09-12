using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IOrganizationServices
    {
        Task<IList<Organization>> GetOrganizationsAsync(bool hierarchical);
        Task<Organization> GetOrganizationAsync(Guid customerId, bool includePreferences, bool includeLocation);
        Task<Location> GetLocationAsync(Guid locationId);
        Task<OrganizationPreferences> RemoveOrganizationPreferencesAsync(Guid organizationId);
        Task<Organization> UpdateOrganizationAsync(Organization updateOrganization);
        Task<IList<AssetCategoryLifecycleType>> RemoveAssetCategoryLifecycleTypesForCustomerAsync(Organization customer, AssetCategoryType assetCategory, IList<AssetCategoryLifecycleType> assetCategoryLifecycleTypes);
        Task<AssetCategoryType> GetAssetCategoryType(Guid customerId, Guid assetCategoryId);
        Task<IList<AssetCategoryType>> GetAssetCategoryTypes(Guid customerId);
        Task<AssetCategoryType> AddAssetCategoryType(Guid customerId, Guid addedAssetCategoryId, IList<int> lifecycleTypes);
        Task<AssetCategoryType> RemoveAssetCategoryType(Guid customerId, Guid deletedAssetCategoryId, IList<int> lifecycleTypes);

        Task<IList<ProductModule>> GetCustomerProductModulesAsync(Guid customerId);
        Task<ProductModule> AddProductModulesAsync(Guid customerId, Guid moduleId, IList<Guid> productModuleGroupIds);
        Task<ProductModule> RemoveProductModulesAsync(Guid customerId, Guid moduleId, IList<Guid> productModuleGroupIds);
        Task<Organization> AddOrganizationAsync(Organization newOrganization);
        Task<OrganizationPreferences> AddOrganizationPreferencesAsync(OrganizationPreferences organizationPreferences);
        Task<Location> AddOrganizationLocationAsync(Location location);
    }
}