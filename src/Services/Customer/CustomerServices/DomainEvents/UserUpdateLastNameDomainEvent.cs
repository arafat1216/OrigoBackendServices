using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class UserUpdateLastNameDomainEvent : BaseEvent
    {
        public UserUpdateLastNameDomainEvent(User user, string oldName) : base(user.UserId)
        {
            User = user;
            OldLastName = oldName;
        }

        public User User { get; protected set; }

        public string OldLastName { get; protected set; }

        public override string EventMessage()
        {
            return $"User last name changed from {OldLastName} to {User.LastName}.";
        }
    }
}
