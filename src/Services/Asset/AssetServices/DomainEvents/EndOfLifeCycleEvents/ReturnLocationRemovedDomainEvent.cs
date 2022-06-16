using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents.EndOfLifeCycleEvents
{
    class ReturnLocationRemovedDomainEvent : BaseEvent
    {
        public ReturnLocationRemovedDomainEvent(ReturnLocation location, Guid callerId, Guid customerId) : base(location.ExternalId)
        {
            Location = location;
            CustomerId = customerId;
            CallerId = callerId;
        }
        public ReturnLocation Location { get; protected set; }
        public Guid CallerId { get; protected set; }
        public Guid CustomerId { get; protected set; }
        public override string EventMessage()
        {
            return $"Return Location Id: {Location.ExternalId} is removed by Caller Id {CallerId} For CustomerId {CustomerId}";
        }

    }
}
