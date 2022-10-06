using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    class SetOnboardingTilesDomainEvent : BaseEvent
    {
        public SetOnboardingTilesDomainEvent(User user, Guid callerId) : base(user.UserId)
        {
            User = user;
            CallerId = callerId;
        }

        public User User { get; protected set; }
        public Guid CallerId { get; protected set; }

        public override string EventMessage()
        {
            return $"Offboarding Tiles Initiated for User {User.Id} by {CallerId}";
        }
    }
}
