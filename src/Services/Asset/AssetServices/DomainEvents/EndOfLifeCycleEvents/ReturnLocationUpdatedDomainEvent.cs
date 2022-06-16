using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents.EndOfLifeCycleEvents
{
    class ReturnLocationUpdatedDomainEvent : BaseEvent
    {

        public ReturnLocationUpdatedDomainEvent(ReturnLocation returnlocation, Guid callerId, ReturnLocation returnLocationData) : base(returnlocation.ExternalId)
        {
            ReturnLocation = returnlocation;
            CallerId = callerId;
            ReturnLocationData = returnLocationData;
        }
        public ReturnLocation ReturnLocation { get; protected set; }
        public Guid CallerId { get; protected set; }
        public ReturnLocation ReturnLocationData { get; protected set; }

        public override string EventMessage()
        {
            return $"Return Location Id: {ReturnLocation.ExternalId} is Updated By Caller Id:{CallerId};" +
                $"{(ReturnLocation.Name == ReturnLocationData.Name ? "" : $"Name from '{ReturnLocation.Name}' To '{ReturnLocationData.Name}';")}" +
                $"{(ReturnLocation.ReturnDescription == ReturnLocationData.ReturnDescription ? "" : $"Return Description from '{ReturnLocation.ReturnDescription}' To '{ReturnLocationData.ReturnDescription}';")}" +
                $"{(ReturnLocation.LocationId == ReturnLocationData.LocationId ? "" : $"LocationId from '{ReturnLocation.LocationId}' To '{ReturnLocationData.LocationId}';")}";
        }

    }
}
