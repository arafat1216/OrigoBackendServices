
using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
#nullable enable
    /// <summary>
    /// Domain event for when a user status change from Invited to onboarding status.
    /// This will be called when the class method from User entity OnboardingInitiated changes the users status.
    /// </summary>
    /// <inheritdoc cref="Common.Logging.BaseEvent" />
    public class UserOnboardingInitiatedDomainEvent : BaseEvent
    {
        /// <summary>
        /// Initializes a new instance of the UserOnboardingInitiatedDomainEvent
        /// <param name="user">User object</param>
        /// <param name="user.UserId">UserId is set by the parent</param>
        public UserOnboardingInitiatedDomainEvent(User user) : base(user.UserId)
        {
            User = user;
        }
        /// <summary>
        /// The user object that is set to onboarding initiated. Object is deserialized when used by the auditlog.
        /// </summary>
        public User User { get; protected set; }

        /// <summary>
        /// Event message used by the auditlog.
        /// </summary>
        /// <inheritdoc cref="Common.Logging.BaseEvent" />
        public override string EventMessage()
        {
            return User.FirstName != null && User.LastName != null ?
             $"{User.FirstName} {User.LastName} has started onboarding process." :
             $"User has started onboarding process.";
        }
    }
}
