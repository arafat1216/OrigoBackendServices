using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    public class CustomerStartedOnboardingDomainEvent : BaseEvent
    {
        public CustomerStartedOnboardingDomainEvent(Organization organization) : base(organization.OrganizationId)
        {
            Organization = organization;
        }

        public Organization Organization { get; protected set; }


        public override string EventMessage()
        {
            return $"Customer has started onboarding.";
        }
    }
}
