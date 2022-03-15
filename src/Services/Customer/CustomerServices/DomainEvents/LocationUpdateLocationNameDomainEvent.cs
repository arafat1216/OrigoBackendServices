using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class LocationUpdateNameDomainEvent : BaseEvent
    {
        public LocationUpdateNameDomainEvent(Location location, string oldName) : base(location.LocationId)
        {
            Location = location;
            OldName = oldName;
        }

        public Location Location { get; protected set; }
        public string OldName { get; protected set; }

        public override string EventMessage()
        {
            return $"Location name changed from {OldName} to {Location.Name}.";
        }
    }
}
