using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class UserDeletedDomainEvent : BaseEvent
    {
        public UserDeletedDomainEvent(User deletedUser) : base(deletedUser.UserId)
        {
            User = deletedUser;
        }

        public User User { get; protected set; }

        public override string EventMessage()
        {
            return $"User {Id} deleted.";
        }
    }
}
