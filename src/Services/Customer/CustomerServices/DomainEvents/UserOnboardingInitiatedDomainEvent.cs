
using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    #nullable enable
    /// <summary>
    /// Domain event for when a user status change from Invited to onboarding status.
    /// This will be called when the class method from User entity OnboardingInitiated changes the users status.
    /// </summary>
    public class UserOnboardingInitiatedDomainEvent : BaseEvent
    {
        public UserOnboardingInitiatedDomainEvent(User user) : base(user.UserId)
        {
            User = user;
        }
        public User User { get; protected set; }

        public override string EventMessage()
        {
            return User.FirstName != null && User.LastName != null ?
             $"{User.FirstName} {User.LastName} has started onboarding process." :
             $"User has started onboarding process.";
        }
    }
}
