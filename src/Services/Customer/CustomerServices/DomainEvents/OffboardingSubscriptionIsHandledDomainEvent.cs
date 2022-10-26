using Common.Logging;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.DomainEvents
{
    public class OffboardingSubscriptionIsHandledDomainEvent : BaseEvent
    {
        public OffboardingSubscriptionIsHandledDomainEvent(UserPreference userPreference, Guid callerId)
        {
            UserPreference = userPreference;
            CallerId = callerId;
        }

        public UserPreference UserPreference { get; protected set; }
        public Guid CallerId { get; protected set; }

        public override string EventMessage()
        {
            return $"Subscription is handled as a part of the offboarding process.";
        }
    }
}
