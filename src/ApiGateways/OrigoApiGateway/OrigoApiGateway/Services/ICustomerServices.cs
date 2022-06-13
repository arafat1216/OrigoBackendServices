using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface ICustomerServices
    {
        Task<IList<Organization>> GetCustomersAsync(Guid? partnerId = null);
        Task<Organization> GetCustomerAsync(Guid customerId);
        Task<IList<CustomerUserCount>> GetCustomerUsersAsync();
        Task<Organization> CreateCustomerAsync(NewOrganization newCustomer, Guid callerId);
        Task<Organization> UpdateOrganizationAsync(UpdateOrganizationDTO organizationToChange);
        Task<Organization> PatchOrganizationAsync(UpdateOrganizationDTO organizationToChange);
        Task<Organization> DeleteOrganizationAsync(Guid organizationId, Guid callerId);

        Task<string> CreateOrganizationSeedData();

        Task<string> GetOktaUserProfileByEmail(string email);
        Task<bool> CheckAndProvisionWebShopUser(string email, string orgnumber);
        Task<Organization> CreatePartnerOrganization(NewOrganization newCustomer, Guid callerId);
        Task<string> GetCurrencyByCustomer(Guid customerId);
        Task<IList<Location>> GetAllCustomerLocations(Guid customerId);
        Task<Location> CreateLocationAsync(OfficeLocation officeLocation, Guid customerId, Guid callerId);
    }
}