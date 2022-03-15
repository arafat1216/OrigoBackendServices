using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class LocationUpdateCityDomainEvent : BaseEvent
    {
        public LocationUpdateCityDomainEvent(Location location, string oldCity) : base(location.LocationId)
        {
            Location = location;
            OldCity = oldCity;
        }

        public Location Location { get; protected set; }
        public string OldCity { get; protected set; }

        public override string EventMessage()
        {
            return $"Location city changed from {OldCity} to {Location.City}.";
        }
    }
}
