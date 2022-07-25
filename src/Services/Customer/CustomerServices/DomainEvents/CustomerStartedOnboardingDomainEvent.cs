using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    public class CustomerStartedOnboardingDomainEvent : BaseEvent
    {
        public CustomerStartedOnboardingDomainEvent(Organization organization, Guid callerId) : base(organization.OrganizationId)
        {
            Organization = organization;
            CallerId = callerId;
        }

        public Organization Organization { get; protected set; }
        public Guid CallerId { get; protected set; }


        public override string EventMessage()
        {
            return $"Customer has started onboarding.";
        }
    }
}
