using CustomerServices.ServiceModels;
using System;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IOktaServices
    {
        Task<string> AddOktaUserAsync(Guid? mytosSubsGuid, string firstName, string lastName, string email, string mobilePhone, bool activate, string countryCode = "+47");
        Task RemoveUserFromGroupAsync(string userOktaId);
        Task AddUserToGroup(string userOktaId);
        Task<bool> UserExistsInOktaAsync(string userOktaId);
        Task<bool> UserHasAppLinks(string userOktaId);
        Task DeleteUserInOkta(string userOktaId);
        Task DeactivateUserInOkta(string userOktaId);
        //https://developer.okta.com/docs/reference/api/users/#response-example-12
        Task<OktaUserDTO> GetOktaUserProfileByLoginEmailAsync(string userOktaId);
    }
}
