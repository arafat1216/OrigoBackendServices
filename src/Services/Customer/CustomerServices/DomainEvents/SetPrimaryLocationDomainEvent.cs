using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class SetPrimaryLocationDomainEvent : BaseEvent
    {
        public SetPrimaryLocationDomainEvent(Location location) : base(location.ExternalId)
        {
            Location = location;
        }

        public Location Location { get; protected set; }

        public override string EventMessage()
        {
            return $"Location with location id {Location.ExternalId} is set as Primary Location for this Orgaziation.";
        }
    }
}
