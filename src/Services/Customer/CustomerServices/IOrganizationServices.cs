using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IOrganizationServices
    {
        Task<IList<Organization>> GetOrganizationsAsync(bool hierarchical = false, bool customersOnly = false);
        Task<IList<CustomerUserCount>> GetCustomerUsersAsync();
        Task<Organization> GetOrganizationAsync(Guid customerId, bool includePreferences = false, bool includeLocation = false, bool onlyCustomer = false);
        Task<Organization> UpdateOrganizationAsync(Organization updateOrganization, bool usingPatch = false);
        Task<Organization> PutOrganizationAsync(Guid organizationId, Guid? parentId, Guid? primaryLocation, Guid callerId, string name, string organizationNumber,
                                                               string street, string postCode, string city, string country,
                                                               string firstName, string lastName, string email, string phoneNumber);
        Task<Organization> PatchOrganizationAsync(Guid organizationId, Guid? parentId, Guid? primaryLocation, Guid callerId, string name, string organizationNumber,
                                                               string street, string postCode, string city, string country,
                                                               string firstName, string lastName, string email, string phoneNumber);
        Task<Organization> DeleteOrganizationAsync(Guid organizationId, Guid callerId, bool hardDelete = false);
        Task<OrganizationPreferences> GetOrganizationPreferencesAsync(Guid organizationId);
        Task<OrganizationPreferences> UpdateOrganizationPreferencesAsync(OrganizationPreferences preferences, bool usingPatch = false);
        Task<OrganizationPreferences> DeleteOrganizationPreferencesAsync(Guid organizationId, Guid callerId, bool hardDelete = false);
        Task<OrganizationPreferences> RemoveOrganizationPreferencesAsync(Guid organizationId);
        Task<Location> GetLocationAsync(Guid locationId);
        Task<Location> UpdateOrganizationLocationAsync(Location updateLocation, bool usingPatch = false);
        Task<Location> DeleteOrganizationLocationAsync(Guid locationId, Guid callerId, bool hardDelete = false);
        Task<IList<AssetCategoryLifecycleType>> RemoveAssetCategoryLifecycleTypesForCustomerAsync(Organization customer, AssetCategoryType assetCategory, IList<AssetCategoryLifecycleType> assetCategoryLifecycleTypes,Guid callerId);
        Task<AssetCategoryType> GetAssetCategoryType(Guid customerId, Guid assetCategoryId);
        Task<AssetCategoryType> GetAssetCategoryType(Guid customerId, int assetCategoryId);
        Task<IList<AssetCategoryType>> GetAssetCategoryTypes(Guid customerId);
        Task<AssetCategoryType> AddAssetCategoryType(Guid customerId, Guid addedAssetCategoryId, IList<int> lifecycleTypes, Guid callerId);
        Task<AssetCategoryType> RemoveAssetCategoryType(Guid customerId, Guid deletedAssetCategoryId, IList<int> lifecycleTypes, Guid callerId);
        Task<IList<ProductModule>> GetCustomerProductModulesAsync(Guid customerId);
        Task<ProductModule> AddProductModulesAsync(Guid customerId, Guid moduleId, IList<Guid> productModuleGroupIds, Guid callerId);
        Task<ProductModule> RemoveProductModulesAsync(Guid customerId, Guid moduleId, IList<Guid> productModuleGroupIds, Guid callerId);
        Task<Organization> AddOrganizationAsync(Organization newOrganization);
        Task<OrganizationPreferences> AddOrganizationPreferencesAsync(OrganizationPreferences organizationPreferences);
        Task<Location> AddOrganizationLocationAsync(Location location);
        Task<bool> ParentOrganizationIsValid(Guid? parentId, Guid organizationId);
        Task<Organization> GetOrganizationByOrganizationNumberAsync(string organizationNumber);
    }
}