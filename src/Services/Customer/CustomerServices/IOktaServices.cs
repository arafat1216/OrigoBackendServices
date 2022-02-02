using System;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IOktaServices
    {
        Task<string> AddOktaUserAsync(Guid? mytosSubsGuid, string firstName, string lastName, string email, string mobilePhone, bool activate, string countryCode = "+47");
        Task RemoveUserFromGroup(string userOktaId);
        Task AddUserToGroup(string userOktaId);
        Task<bool> UserExistsInOktaAsync(string userOktaId);
        Task<bool> UserHasAppLinks(string userOktaId);
        Task DeleteUserInOkta(string userOktaId);
        Task DeactivateUserInOkta(string userOktaId);
        Task<string> GetOktaUserProfileByLoginEmailAsync(string userOktaId);
    }
}
