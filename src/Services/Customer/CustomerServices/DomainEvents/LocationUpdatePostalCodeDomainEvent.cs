using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class LocationUpdatePostalCodeDomainEvent : BaseEvent
    {
        public LocationUpdatePostalCodeDomainEvent(Location location, string oldPostalCode) : base(location.LocationId)
        {
            Location = location;
            OldPostalCode = oldPostalCode;
        }

        public Location Location { get; protected set; }
        public string OldPostalCode { get; protected set; }

        public override string EventMessage()
        {
            return $"Location postal code changed from {OldPostalCode} to {Location.PostalCode}.";
        }
    }
}
