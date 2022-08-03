using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    class OffboardingCancelledDomainEvent : BaseEvent
    {
        public OffboardingCancelledDomainEvent(User user, Guid callerId) : base(user.UserId)
        {
            User = user;
            CallerId = callerId;
        }

        public User User { get; protected set; }
        public Guid CallerId { get; protected set; }

        public override string EventMessage()
        {
            return $"Offboarding Cancelled for User {User.Id} by {CallerId} and User Status changed to 'Activated'";
        }

    }
}
