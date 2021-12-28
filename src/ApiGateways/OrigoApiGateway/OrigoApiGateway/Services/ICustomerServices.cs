using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface ICustomerServices
    {
        Task<IList<Organization>> GetCustomersAsync();
        Task<Organization> GetCustomerAsync(Guid customerId);
        Task<Organization> CreateCustomerAsync(NewOrganizationDTO newCustomer);
        Task<Organization> UpdateOrganizationAsync(UpdateOrganization organizationToChange);
        Task<Organization> PatchOrganizationAsync(UpdateOrganization organizationToChange);
        Task<Organization> DeleteOrganizationAsync(Guid organizationId, Guid callerId);

        Task<IList<OrigoCustomerAssetCategoryType>> GetAssetCategoryForCustomerAsync(Guid customerId);
        Task<OrigoCustomerAssetCategoryType> AddAssetCategoryForCustomerAsync(Guid customerId, NewCustomerAssetCategoryTypeDTO customerAssetCategoryType);
        Task<OrigoCustomerAssetCategoryType> RemoveAssetCategoryForCustomerAsync(Guid customerId, NewCustomerAssetCategoryTypeDTO customerAssetCategoryType);

        Task<IList<OrigoProductModule>> GetCustomerProductModulesAsync(Guid customerId);
        Task<OrigoProductModule> AddProductModulesAsync(Guid customerId, NewCustomerProductModuleDTO productModule);
        Task<OrigoProductModule> RemoveProductModulesAsync(Guid customerId, NewCustomerProductModuleDTO productModule);
    }
}