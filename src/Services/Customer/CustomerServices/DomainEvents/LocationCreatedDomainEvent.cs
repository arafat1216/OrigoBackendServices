﻿using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class LocationCreatedDomainEvent : BaseEvent
    {

        public LocationCreatedDomainEvent(Location location) : base(location.LocationId)
        {
            Location = location;
        }
        public Location Location { get; protected set; }
        public override string EventMessage()
        {
            return $"Location created {Location.LocationId}.";
        }
       
    }
}

