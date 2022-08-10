
using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
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
