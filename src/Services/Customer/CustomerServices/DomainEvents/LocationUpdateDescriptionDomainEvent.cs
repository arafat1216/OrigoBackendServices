using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class LocationUpdateDescriptionDomainEvent : BaseEvent
    {
        public LocationUpdateDescriptionDomainEvent(Location location, string oldDescription) : base(location.LocationId)
        {
            Location = location;
            OldDescription = oldDescription;
        }
        public Location Location { get; protected set; }
        public string OldDescription { get; protected set; }
        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Location description changed from {OldDescription} to {Location.Description}.";
        }
    }
}
