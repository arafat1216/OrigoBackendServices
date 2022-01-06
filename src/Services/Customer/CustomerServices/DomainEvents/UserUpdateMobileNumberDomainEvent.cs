using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    internal class UserUpdateMobileNumberDomainEvent : BaseEvent
    {
        public UserUpdateMobileNumberDomainEvent(User user, string oldMobileNumber, Guid callerId) : base(user.UserId)
        {
            User = user;
            OldMobileNumber = oldMobileNumber;
            CallerId = callerId;
        }
        public User User { get; protected set; }
        public string OldMobileNumber { get; protected set; }
        public Guid CallerId { get; protected set; }
        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"User mobile phone number changed from {OldMobileNumber} to {User.MobileNumber}.";
        }
    }
}
