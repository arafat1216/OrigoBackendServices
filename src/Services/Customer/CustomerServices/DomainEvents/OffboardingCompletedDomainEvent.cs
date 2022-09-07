using Common.Logging;
using CustomerServices.Models;
using System;


namespace CustomerServices.DomainEvents
{
    class OffboardingCompletedDomainEvent : BaseEvent
    {
        public OffboardingCompletedDomainEvent(User user, Guid callerId) : base(user.UserId)
        {
            User = user;
            CallerId = callerId;
        }

        public User User { get; protected set; }
        public Guid CallerId { get; protected set; }

        public override string EventMessage()
        {
            return $"Offboarding completed for User {User.Id}; User Status changed to 'OffboardCompleted'";
        }

    }

}
