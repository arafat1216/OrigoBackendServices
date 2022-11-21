using Common.Interfaces;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.SCIM;

namespace OrigoApiGateway.Services;

public interface IScimService
{
    Task<User> GetUserAsync(Guid userId);

    Task<ListResponse<User>> GetAllUsersAsync(CancellationToken cancellationToken, string userName = "", int page = 1, int limit = 25);

    Task<User> AddUserForCustomerAsync(Guid customerId, NewUser newUser, Guid callerId, bool includeOnboarding);

    Task<User> PutUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser, Guid callerId);

    Task<User> DeleteUserAsync(Guid userId, bool softDelete, Guid callerId);
}