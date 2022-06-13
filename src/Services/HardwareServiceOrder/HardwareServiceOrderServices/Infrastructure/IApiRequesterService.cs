using Microsoft.EntityFrameworkCore;

namespace HardwareServiceOrderServices.Infrastructure
{
    /// <summary>
    ///     A service/dependency-injected interface that details what should be intercepted and stored from the APIs incoming HTTP-request,
    ///     allowing it to be used elsewhere in the call-stack, such as by a <see cref="DbContext"/>.
    /// </summary>
    public interface IApiRequesterService
    {
        /// <summary>
        ///     The ID of the authenticated user that made the API call.
        /// </summary>
        public Guid AuthenticatedUserId { get; }
    }
}
