using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class LocationRemovedToCustomerDomainEvent : BaseEvent
    {
        public LocationRemovedToCustomerDomainEvent(Location location) : base(location.ExternalId)
        {
            Location = location;
        }

        public Location Location { get; protected set; }

        public override string EventMessage()
        {
            return $"Location {Id} removed.";
        }
    }
}
