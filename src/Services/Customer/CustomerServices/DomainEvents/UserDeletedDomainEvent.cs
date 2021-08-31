using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class UserDeletedDomainEvent : BaseEvent
    {
        public UserDeletedDomainEvent(User deletedUser) : base(deletedUser.UserId)
        {
            NewUser = deletedUser;
        }

        public User NewUser { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"User {Id} deleted.";
        }
    }
}
