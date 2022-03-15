using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class UserUpdateEmailDomainEvent : BaseEvent
    {
        public UserUpdateEmailDomainEvent(User user, string oldEmail) : base(user.UserId)
        {
            User = user;
            OldEmail = oldEmail;
        }

        public User User { get; protected set; }
        public string OldEmail { get; protected set; }

        public override string EventMessage()
        {
            return $"User email changed from {OldEmail} to {User.Email}.";
        }
    }
}
