using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents
{
    public class SetRuntimeDomainEvent: BaseEvent
    {
        public SetRuntimeDomainEvent(LifeCycleSetting lifeCycleSetting, Guid customerId, Guid callerId, int previousRuntime) : base()
        {
            LifeCycleSetting = lifeCycleSetting;
            CallerId = callerId;
            CustomerId = customerId;
            PreviousRuntime = previousRuntime;
        }

        public LifeCycleSetting LifeCycleSetting { get; protected set; }
        public Guid CallerId { get; protected set; }
        public Guid CustomerId { get; protected set; }
        public int PreviousRuntime { get; protected set; }

        public override string EventMessage()
        {
            return $"Runtime has been set From '{PreviousRuntime}' months To '{LifeCycleSetting.Runtime}' months; for CustomerId: {LifeCycleSetting.CustomerId}; AssetCategory: {LifeCycleSetting.AssetCategoryName};";
        }
    }
}
