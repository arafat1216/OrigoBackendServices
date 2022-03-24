using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    internal class OrganizationPartnerChangedDomainEvent : BaseEvent
    {
        private Organization Organization { get; set; }
        private Guid? OldPartnerId { get; set; }

        public OrganizationPartnerChangedDomainEvent(Organization organization, Guid? oldPartnerId) : base(organization.OrganizationId)
        {
            Organization = organization;
            OldPartnerId = oldPartnerId;
        }

        public override string EventMessage()
        {
            return $"Partner changed from {OldPartnerId} to {Organization.Partner.ExternalId}.";
        }
    }
}
