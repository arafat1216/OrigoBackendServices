﻿using Common.Cryptography;
using Common.Enums;
using CustomerServices.Exceptions;
using CustomerServices.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CustomerServices
{
    public class OrganizationServices : IOrganizationServices
    {
        private readonly ILogger<OrganizationServices> _logger;
        private readonly IOrganizationRepository _customerRepository;

        public OrganizationServices(ILogger<OrganizationServices> logger, IOrganizationRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Returns all organizations. If hierarchical is true, return the organizations with no parent organization, these organizations will have their child organizations appended to them as a list.
        /// </summary>
        /// <param name="hierarchical"></param>
        /// <returns></returns>
        public async Task<IList<Organization>> GetOrganizationsAsync(bool hierarchical = false)
        {
            if (!hierarchical)
            {

                var orgs = await _customerRepository.GetOrganizationsAsync();
                foreach (Organization o in orgs)
                {
                    o.OrganizationPreferences = await _customerRepository.GetOrganizationPreferencesAsync(o.OrganizationId);
                    o.OrganizationLocation = await _customerRepository.GetOrganizationLocationAsync(o.PrimaryLocation);
                }
                return orgs;
            }
            else
            {
                Guid? p = Guid.Empty;
                var organizations = await _customerRepository.GetOrganizationsAsync(p);
                foreach (Organization o in organizations)
                {
                    o.ChildOrganizations = await _customerRepository.GetOrganizationsAsync(o.OrganizationId);
                    o.OrganizationPreferences = await _customerRepository.GetOrganizationPreferencesAsync(o.OrganizationId);
                    o.OrganizationLocation = await _customerRepository.GetOrganizationLocationAsync(o.PrimaryLocation);
                }
                return organizations;
            }
        }

        /// <summary>
        /// Returns all Organization entities with the given ParentId. If the ParentId is null, return all Organizations that do not have parent entities.
        /// </summary>
        /// <param name="parentId">Guid value that points to the ExternalId of an Organization</param>
        /// <returns>A list of Organizations</returns>
        public async Task<IList<Organization>> GetOrganizationsByParentId(Guid? parentId)
        {
            return await _customerRepository.GetOrganizationsAsync(parentId);
        }

        public async Task<Organization> GetOrganizationAsync(Guid customerId)
        {
            return await _customerRepository.GetOrganizationAsync(customerId);
        }

        /// <summary>
        /// Get the organization with the given Id. Optional: Return the OrganizationPreferences and OrganizationLocation object of the organization along with the organization itself.
        /// </summary>
        /// <param name="customerId">The id of the organization queried</param>
        /// <param name="includePreferences">Include OrganizationPreferences object of the organization if set to true</param>
        /// <param name="includeLocation">Include OrganizationLocation object of the organization if set to true</param>
        /// <returns>Organization</returns>
        public async Task<Organization> GetOrganizationAsync(Guid customerId, bool includePreferences = false, bool includeLocation = false)
        {
            var organization = await _customerRepository.GetOrganizationAsync(customerId);

            if (organization != null)
            {
                if (includePreferences)
                {
                    organization.OrganizationPreferences = await _customerRepository.GetOrganizationPreferencesAsync(customerId);
                }
                if (includeLocation)
                {
                    organization.OrganizationLocation = await _customerRepository.GetOrganizationLocationAsync(organization.PrimaryLocation);
                }
            }

            return organization;
        }

        /// <summary>
        /// Add the given Organization to the database.
        /// </summary>
        /// <param name="newOrganization">An Organization entity, to be added to the database</param>
        /// <returns></returns>
        public async Task<Organization> AddOrganizationAsync(Organization newOrganization)
        {
            return await _customerRepository.AddAsync(newOrganization);
        }

        public async Task<Organization> UpdateOrganizationAsync(Organization updateOrganization, bool usingPatch = false)
        {
            try
            {
                var organization = await _customerRepository.GetOrganizationAsync(updateOrganization.OrganizationId);
                if (usingPatch)
                    organization.PatchOrganization(organization.ParentId, organization.PrimaryLocation,
                                                 organization.OrganizationName, organization.OrganizationNumber);
                else
                    organization.UpdateOrganization(organization.ParentId, organization.PrimaryLocation,
                                                    organization.OrganizationName, organization.OrganizationNumber);


                await _customerRepository.SaveEntitiesAsync();

                return organization;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - UpdateOrganizationAsync failed to update: " + ex.Message);
                throw;
            }
        }

        public async Task<Organization> DeleteOrganizationAsync(Guid organizationId, Guid callerId, bool hardDelete = false)
        {
            try
            {
                var organization = await _customerRepository.GetOrganizationAsync(organizationId);

                if (organization == null)
                    throw new CustomerNotFoundException();

                if (organization.IsDeleted && !hardDelete)
                    throw new CustomerNotFoundException();

                if (organization.PrimaryLocation != null)
                {
                    await DeleteOrganizationLocationAsync((Guid)organization.PrimaryLocation, callerId, hardDelete);
                }



                await DeleteOrganizationPreferencesAsync(organizationId, callerId, hardDelete);



                // set IsDelete, caller and date of change
                organization.Delete(callerId);
                await _customerRepository.SaveEntitiesAsync();

                // Complete delete, removed from database
                if (hardDelete)
                {
                    return await _customerRepository.DeleteOrganizationAsync(organization);
                }

                return organization;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices -DeleteOrganizationAsync failed to delete: " + ex.Message);
                throw;
            }
        }

        public async Task<OrganizationPreferences> GetOrganizationPreferencesAsync(Guid organizationId)
        {
            try
            {
                var preferences = await _customerRepository.GetOrganizationPreferencesAsync(organizationId);
                if (preferences == null)
                    return null;

                if (preferences.IsDeleted)
                    throw new EntityIsDeletedException();
                return preferences;
            }
            catch (EntityIsDeletedException ex)
            {
                _logger.LogError("Entity is deleted. {0}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices -GetOrganizationPreferences failed to be retrieved: " + ex.Message);
                throw;
            }
        }

        public async Task<OrganizationPreferences> AddOrganizationPreferencesAsync(OrganizationPreferences organizationPreferences)
        {
            return await _customerRepository.AddOrganizationPreferencesAsync(organizationPreferences);
        }

        public async Task<OrganizationPreferences> UpdateOrganizationPreferencesAsync(OrganizationPreferences preferences, bool usingPatch = false)
        {
            try
            {
                var currentPreferences = await _customerRepository.GetOrganizationPreferencesAsync(preferences.OrganizationId);
                if (currentPreferences == null)
                    return null;

                if (usingPatch)
                    currentPreferences.PatchPreferences(preferences);
                else
                    currentPreferences.UpdatePreferences(preferences);

                await _customerRepository.SaveEntitiesAsync();
                return currentPreferences;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - UpdateOrganizationPreferencesAsync failed to update: " + ex.Message);
                throw;
            }
        }

        public async Task<OrganizationPreferences> DeleteOrganizationPreferencesAsync(Guid organizationId, Guid callerId, bool hardDelete = false)
        {
            try
            {
                var preferences = await _customerRepository.GetOrganizationPreferencesAsync(organizationId);
                if (preferences == null) // object is already deleted
                    return null;
                preferences.Delete(callerId);
                await _customerRepository.SaveEntitiesAsync();

                if (hardDelete)
                {
                    return await _customerRepository.DeleteOrganizationPreferencesAsync(preferences);
                }

                return preferences;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - DeleteOrganizationPreferencesAsync failed to delete: " + ex.Message);
                throw;
            }
        }

        public async Task<OrganizationPreferences> RemoveOrganizationPreferencesAsync(Guid organizationId)
        {
            var organizationPreferences = await _customerRepository.GetOrganizationPreferencesAsync(organizationId);
            return await _customerRepository.DeleteOrganizationPreferencesAsync(organizationPreferences);
        }

        /// <summary>
        /// Get the location object with the given Id
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns>Null or Location</returns>
        public async Task<Location> GetLocationAsync(Guid locationId)
        {
            return await _customerRepository.GetOrganizationLocationAsync(locationId);
        }

        public async Task<Location> AddOrganizationLocationAsync(Location location)
        {
            return await _customerRepository.AddOrganizationLocationAsync(location);
        }

        public async Task<Location> UpdateOrganizationLocationAsync(Location updateLocation, bool usingPatch = false)
        {
            try
            {
                var currentLocation = await _customerRepository.GetOrganizationLocationAsync(updateLocation.LocationId);
                if (currentLocation == null)
                {
                    return null;
                }

                if (usingPatch)
                    currentLocation.PatchLocation(updateLocation);
                else
                    currentLocation.UpdateLocation(updateLocation);

                await _customerRepository.SaveEntitiesAsync();
                return currentLocation;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - UpdateOrganizationLocationAsync failed to update: " + ex.Message);
                throw;
            }
        }

        public async Task<Location> DeleteOrganizationLocationAsync(Guid locationId, Guid callerId, bool hardDelete = false)
        {
            try
            {
                var location = await _customerRepository.GetOrganizationLocationAsync(locationId);
                if (location == null) // object is already deleted
                    return null;
                location.Delete(callerId);
                await _customerRepository.SaveEntitiesAsync();

                if (hardDelete)
                {
                    return await _customerRepository.DeleteOrganizationLocationAsync(location);
                }

                return location;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - DeleteOrganizationLocationAsync failed to delete: " + ex.Message);
                throw;
            }
        }

        public async Task<IList<AssetCategoryLifecycleType>> RemoveAssetCategoryLifecycleTypesForCustomerAsync(Organization customer, AssetCategoryType assetCategory, IList<AssetCategoryLifecycleType> assetCategoryLifecycleTypes)
        {
            return await _customerRepository.DeleteAssetCategoryLifecycleTypeAsync(customer, assetCategory, assetCategoryLifecycleTypes);
        }

        public async Task<AssetCategoryType> GetAssetCategoryType(Guid customerId, Guid assetCategoryId)
        {
            return await _customerRepository.GetAssetCategoryTypeAsync(customerId, assetCategoryId);
        }

        public async Task<IList<AssetCategoryType>> GetAssetCategoryTypes(Guid customerId)
        {
            var customerCategories = await _customerRepository.GetAssetCategoryTypesAsync(customerId);
            return customerCategories;
        }

        public async Task<AssetCategoryType> AddAssetCategoryType(Guid customerId, Guid addedAssetCategoryId, IList<int> lifecycleTypes)
        {
            var customer = await GetOrganizationAsync(customerId);
            var assetCategory = await GetAssetCategoryType(customerId, addedAssetCategoryId);
            if (customer == null)
            {
                return null;
            }
            if (assetCategory != null)
            {
                assetCategory.SetAssetCategoryId(addedAssetCategoryId);
                foreach (var lifecycle in lifecycleTypes)
                {
                    var exist = assetCategory.LifecycleTypes.FirstOrDefault(a => a.LifecycleType == (LifecycleType)lifecycle);
                    if (exist == null)
                        customer.AddLifecyle(assetCategory, new AssetCategoryLifecycleType(customerId, addedAssetCategoryId, lifecycle));
                }
            }
            else
            {
                assetCategory = new AssetCategoryType(addedAssetCategoryId, customerId, new List<AssetCategoryLifecycleType>());
                customer.AddAssetCategory(assetCategory);
                foreach (int lifecycle in lifecycleTypes)
                {
                    customer.AddLifecyle(assetCategory, new AssetCategoryLifecycleType(customerId, addedAssetCategoryId, lifecycle));
                }
            }
            await _customerRepository.SaveEntitiesAsync();
            // return updated object
            return await GetAssetCategoryType(customerId, addedAssetCategoryId);
        }

        public async Task<AssetCategoryType> RemoveAssetCategoryType(Guid customerId, Guid deletedAssetCategoryId, IList<int> lifecycleTypes)
        {
            var customer = await GetOrganizationAsync(customerId);
            var assetCategory = await GetAssetCategoryType(customerId, deletedAssetCategoryId);
            if (customer == null || assetCategory == null)
            {
                return null;
            }
            // If no lifecycles are selected delete the asset category as well
            if (!lifecycleTypes.Any())
            {
                await RemoveAssetCategoryLifecycleTypesForCustomerAsync(customer, assetCategory, assetCategory.LifecycleTypes.ToList());
                customer.RemoveAssetCategory(assetCategory);
                return await _customerRepository.DeleteAssetCategoryTypeAsync(assetCategory);
            }
            var lifecycles = assetCategory.LifecycleTypes.Where(a => lifecycleTypes.Contains((int)a.LifecycleType)).Select(a => a).ToList();
            // Delete lifecycles of this asset category
            await RemoveAssetCategoryLifecycleTypesForCustomerAsync(customer, assetCategory, lifecycles);
            // return updated object
            return await GetAssetCategoryType(customerId, deletedAssetCategoryId);
        }

        public async Task<IList<ProductModule>> GetCustomerProductModulesAsync(Guid customerId)
        {
            return await _customerRepository.GetCustomerProductModulesAsync(customerId);
        }

        public async Task<ProductModule> AddProductModulesAsync(Guid customerId, Guid moduleId, IList<Guid> productModuleGroupIds)
        {
            var productModule = await _customerRepository.AddProductModuleAsync(customerId, moduleId);
            if (productModule != null)
            {
                foreach (var moduleGroupId in productModuleGroupIds)
                {
                    await _customerRepository.AddProductModuleGroupAsync(customerId, moduleGroupId);
                }
            }
            var result = await GetCustomerProductModulesAsync(customerId);
            return result.FirstOrDefault(m => m.ProductModuleId == moduleId);
        }

        public async Task<ProductModule> RemoveProductModulesAsync(Guid customerId, Guid moduleId, IList<Guid> productModuleGroupIds)
        {
            var customerModules = await GetCustomerProductModulesAsync(customerId);
            var module = customerModules.FirstOrDefault(m => m.ProductModuleId == moduleId);
            if (module == null)
            {
                return null;
            }
            if (!productModuleGroupIds.Any()) // remove module and module groups
            {
                foreach (var moduleGroup in module.ProductModuleGroup)
                {
                    await _customerRepository.RemoveProductModuleGroupAsync(customerId, moduleGroup.ProductModuleGroupId);
                }
                await _customerRepository.RemoveProductModuleAsync(customerId, moduleId);
                return GetCustomerProductModulesAsync(customerId).Result.FirstOrDefault(m => m.ProductModuleId == moduleId);
            }
            foreach (var groupId in productModuleGroupIds) // remove module groups
            {
                await _customerRepository.RemoveProductModuleGroupAsync(customerId, groupId);
            }
            var result = await GetCustomerProductModulesAsync(customerId);
            return result.FirstOrDefault(m => m.ProductModuleId == moduleId);
        }

        public async Task<string> EncryptDataForCustomer(Guid customerId, string message, byte[] secretKey, byte[] iv)
        {
            try
            {
                var customer = await _customerRepository.GetOrganizationAsync(customerId);

                if (customer == null)
                    return null;

                string salt = customer.OrganizationId.ToString();


                var encryptedMessage = Encryption.EncryptData(message, salt, secretKey, iv);

                return encryptedMessage;
            }
            catch (CryptographicException ex)
            {
                _logger.LogError("EncryptDataForCustomer failed with CryptographicException error: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("EncryptDataForCustomer failed with unknown error: " + ex.Message);
                throw;
            }
        }

        public async Task<string> DecryptDataForCustomer(Guid customerId, string encryptedData, byte[] secretKey, byte[] iv)
        {
            try
            {
                var customer = await _customerRepository.GetOrganizationAsync(customerId);
                if (customer == null)
                    return null;

                string salt = customer.OrganizationId.ToString();
                var decryptedMessage = Encryption.DecryptData(encryptedData, salt, secretKey, iv);

                return decryptedMessage;
            }
            catch (CryptographicException ex)
            {
                _logger.LogError("DecryptDataForCustomer failed with CryptographicException error: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("DecryptDataForCustomer failed with unknown error: " + ex.Message);
                throw;
            }
        }
    }
}