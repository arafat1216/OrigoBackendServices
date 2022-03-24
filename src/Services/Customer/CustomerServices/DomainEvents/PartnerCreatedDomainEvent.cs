using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class PartnerCreatedDomainEvent : BaseEvent
    {
        public PartnerCreatedDomainEvent(Partner partner) : base(partner.ExternalId)
        {
            NewPartner = partner;
        }

        public Partner NewPartner { get; protected set; }

        public override string EventMessage()
        {
            return $"Partner {NewPartner.ExternalId} created.";
        }
    }
}
