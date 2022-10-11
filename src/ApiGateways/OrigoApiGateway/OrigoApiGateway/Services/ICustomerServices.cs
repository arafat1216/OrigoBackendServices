using Common.Interfaces;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Models.TechstepCore;

namespace OrigoApiGateway.Services
{
    public interface ICustomerServices
    {
        Task<IList<Organization>> GetCustomersAsync(Guid? partnerId = null, bool includePreferences = true);
        Task<PagedModel<Organization>> GetPaginatedCustomersAsync(CancellationToken cancellationToken, int page, int limit, Guid? partnerId = null, bool includePreferences = true);
        Task<Organization> GetCustomerAsync(Guid customerId);
        Task<IList<CustomerUserCount>> GetCustomerUsersAsync(FilterOptionsForUser filterOptions);
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
        Task<IList<Location>> DeleteLocationAsync(Guid customerId, Guid locationId, Guid callerId);
        Task<Organization> InitiateOnbardingAsync(Guid customerId);
        Task UpdateCustomerFromTechstepCore(TechstepCoreCustomersData techstepCoreUpdate);
        Task<TechstepCustomers> GetTechstepCustomers(string searchString);
    }
}