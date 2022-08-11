
using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
#nullable enable
    /// <summary>
    /// Domain event when user status change from Invited to onboarding status.
    /// </summary>
    public class UserOnboardingInitiatedDomainEvent : BaseEvent
    {
        /// <summary>
        /// Initializes a new instance of the UserOnboardingInitiatedDomainEvent
        /// </summary>
        /// <param name="user">User object</param>
        public UserOnboardingInitiatedDomainEvent(User user) : base(user.UserId)
        {
            User = user;
        }
        /// <summary>
        /// The user object that is set to onboarding initiated. Object is deserialized when used by the auditlog.
        /// </summary>
        public User User { get; protected set; }

        /// <inheritdoc/>
        public override string EventMessage()
        {
            return User.FirstName != null && User.LastName != null ?
             $"{User.FirstName} {User.LastName} has started onboarding process." :
             $"User has started onboarding process.";
        }
    }
}
