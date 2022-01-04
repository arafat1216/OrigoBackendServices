using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class LocationUpdatedDomainEvent : BaseEvent
    {
        public LocationUpdatedDomainEvent(Location location) : base(location.LocationId)
        {
            Location = location;
        }
        public Location Location { get; protected set; }
        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Location with location id {Location.LocationId} was updated.";
        }
    }
}
