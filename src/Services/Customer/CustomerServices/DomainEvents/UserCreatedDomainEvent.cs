using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class UserCreatedDomainEvent : BaseEvent
    {
        public UserCreatedDomainEvent(User newUser) : base(newUser.UserId)
        {
            NewUser = newUser;
        }

        public User NewUser { get; protected set; }

        public override string EventMessage()
        {
            return $"User {Id} created.";
        }
    }
}
