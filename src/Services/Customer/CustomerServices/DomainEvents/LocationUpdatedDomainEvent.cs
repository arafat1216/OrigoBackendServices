using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class LocationUpdatedDomainEvent : BaseEvent
    {
        public LocationUpdatedDomainEvent(Location location) : base(location.ExternalId)
        {
            Location = location;
        }

        public Location Location { get; protected set; }

        public override string EventMessage()
        {
            return $"Location with location id {Location.ExternalId} was updated.";
        }
    }
}
