using Common.Enums;
using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    class UserStatusChangedDomainEvent : BaseEvent
    {
        public UserStatusChangedDomainEvent(User user,Guid callerId, UserStatus oldState) : base(user.UserId)
        {
            User = user;
            OldState = oldState;
            CallerId = callerId;
        }

        public User User { get; protected set; }
        public UserStatus OldState { get; protected set; }
        public Guid CallerId { get; protected set; }


        public override string EventMessage()
        {
            return $"User status changed from {OldState.ToString()} to {User.UserStatus.ToString()}.";
        }
    }
}
