using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class UserActivateDeactivateDomainEvent : BaseEvent
    {
        public UserActivateDeactivateDomainEvent(User user,bool oldState) : base(user.UserId)
        {
            User = user;
            OldState = oldState;
        }

        public User User { get; protected set; }
        public bool OldState { get; protected set; }

        public override string EventMessage()
        {
            return $"User state changed from {OldState.ToString()} to {User.IsActive.ToString()}.";
        }
    }
}
