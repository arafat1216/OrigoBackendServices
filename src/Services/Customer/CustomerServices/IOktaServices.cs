using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IOktaServices
    {
        Task<string> AddOktaUserAsync(Guid? mytosSubsGuid, string firstName, string lastName, string email, string mobilePhone, bool activate, string countryCode = "+47");
        Task RemoveUserFromGroup(string userOktaId);
        Task AddUserToGroup(string userOktaId);
        Task<bool> UserExistsInOkta(string userOktaId);
        Task<bool> UserHasAppLinks(string userOktaId);
        Task DeleteUserInOkta(string userOktaId);
        Task DeactivateUserInOkta(string userOktaId);
    }
}
