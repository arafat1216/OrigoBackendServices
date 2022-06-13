using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    class LocationAddedToCustomerDomainEvent : BaseEvent
    {
        public LocationAddedToCustomerDomainEvent(Location location, Guid customerId) : base(location.ExternalId)
        {
            Location = location;
            CustomerId = customerId;
        }

        public Location Location { get; protected set; }
        public Guid CustomerId { get; protected set; }

        public override string EventMessage()
        {
            return $"Location {Location.ExternalId} added to customer {CustomerId}.";
        }
    }
}
