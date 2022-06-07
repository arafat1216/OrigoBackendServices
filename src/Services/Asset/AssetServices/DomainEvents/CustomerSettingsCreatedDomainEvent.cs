using System;
using AssetServices.Models;
using Common.Logging;


namespace AssetServices.DomainEvents
{
    public class CustomerSettingsCreatedDomainEvent<T> : BaseEvent
    {
        public CustomerSettingsCreatedDomainEvent(CustomerSettings customerSettings, Guid callerId)
        {
            CustomerSettings = customerSettings;
            CallerId = callerId;
        }

        public CustomerSettings CustomerSettings { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage()
        {
            return $"Customer settings with customer id: {CustomerSettings.CustomerId} was created";
        }
    }
}
