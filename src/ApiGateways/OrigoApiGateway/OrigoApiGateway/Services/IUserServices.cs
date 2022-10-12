#nullable enable
using Common.Interfaces;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using System.Security.Claims;

namespace OrigoApiGateway.Services
{
    public interface IUserServices
    {
        Task<CustomerUserCount> GetUsersCountAsync(Guid customerId, FilterOptionsForUser filterOptions);
        Task<OrigoUser> GetUserAsync(Guid customerId, Guid userId);
        Task<OrigoUser> GetUserAsync(Guid userId);
        Task<OrigoMeUser?> GetUserWithPermissionsAsync(Guid? customerId, Guid mainOrganizationId, Guid userId, List<string> permissions,
            List<string> accessList);
        Task<PagedModel<OrigoUser>> GetAllUsersAsync(Guid customerId, FilterOptionsForUser filterOptions, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 1000);
        Task<OrigoUser> AddUserForCustomerAsync(Guid customerId, NewUser newUser, Guid callerId, bool includeOnboarding);
        Task<OrigoUser> PutUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser, Guid callerId);
        Task<OrigoUser> PatchUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser, Guid callerId);
        Task<OrigoUser> DeleteUserAsync(Guid customerId, Guid userId, bool softDelete, Guid callerId);
        Task<OrigoUser> SetUserActiveStatusAsync(Guid customerId, Guid userId, bool isActive, Guid callerId);
        Task<OrigoUser> AssignUserToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task<OrigoUser> UnassignUserFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task<UserInfoDTO> GetUserInfo(string userName, Guid userId);
        Task<OrigoUser> InitiateOffboarding(Guid customerId, Guid userId, string role, List<Guid> departments, OffboardInitiate offboardDate, IList<LifeCycleSetting> lifeCycleSettings, Guid callerId);
        Task<OrigoUser> CancelOffboarding(Guid customerId, Guid userId, string role, List<Guid> departments, Guid callerId);
        Task<OrigoExceptionMessages> ResendOrigoInvitationMail(Guid customerId, InviteUsers users, FilterOptionsForUser filterOptions);
        Task<OrigoUser> CompleteOnboardingAsync(Guid customerId, Guid userId);
        Task<(bool correctOrganization, Guid userId)> UserEmailLinkedToGivenOrganization(Guid organizationId, string userEmail);
    }
}
