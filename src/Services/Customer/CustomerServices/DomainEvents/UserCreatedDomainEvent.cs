using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class UserCreatedDomainEvent : BaseEvent
    {
        public UserCreatedDomainEvent(User newUser) : base(newUser.UserId)
        {
            NewAsset = newUser;
        }

        public User NewAsset { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"User {Id} created.";
        }
    }
}
