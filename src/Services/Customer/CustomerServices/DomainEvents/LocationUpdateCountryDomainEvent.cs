using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class LocationUpdateCountryDomainEvent : BaseEvent
    {
        public LocationUpdateCountryDomainEvent(Location location, string oldCountry) : base(location.LocationId)
        {
            Location = location;
            OldCountry = oldCountry;
        }
        public Location Location { get; protected set; }
        public string OldCountry { get; protected set; }
        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Location country changed from {OldCountry} to {Location.Country}.";
        }
    }
}
