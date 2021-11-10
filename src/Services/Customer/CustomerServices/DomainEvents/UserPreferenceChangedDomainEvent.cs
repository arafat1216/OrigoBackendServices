using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    class UserPreferenceChangedDomainEvent : BaseEvent
    {
        public UserPreferenceChangedDomainEvent(UserPreference userPreference, Guid userId) : base(userId)
        {
            User = userPreference;
            UserId = userId;
        }

        public UserPreference User { get; protected set; }

        public Guid UserId { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"User preferences changed.";
        }
    }
}
