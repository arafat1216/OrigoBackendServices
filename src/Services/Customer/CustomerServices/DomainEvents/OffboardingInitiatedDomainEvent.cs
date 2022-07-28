using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    class OffboardingInitiatedDomainEvent : BaseEvent
    {
        public OffboardingInitiatedDomainEvent(User user, Guid callerId) : base(user.UserId)
        {
            User = user;
            CallerId = CallerId;
        }

        public User User { get; protected set; }
        public User CallerId { get; protected set; }

        public override string EventMessage()
        {
            return $"Offboarding Initiated for User {User.Id} by {CallerId}";
        }

    }
}
