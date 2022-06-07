using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents
{
    public class SetRuntimeDomainEvent: BaseEvent
    {
        public SetRuntimeDomainEvent(LifeCycleSetting lifeCycleSetting, Guid callerId, int previousRuntime) : base(lifeCycleSetting.ExternalId)
        {
            LifeCycleSetting = lifeCycleSetting;
            CallerId = callerId;
            PreviousRuntime = previousRuntime;
        }

        public LifeCycleSetting LifeCycleSetting { get; protected set; }
        public Guid CallerId { get; protected set; }
        public int PreviousRuntime { get; protected set; }

        public override string EventMessage()
        {
            return $"Runtime has been set From '{PreviousRuntime}' months To '{LifeCycleSetting.Runtime}' months; for SettingId: {LifeCycleSetting.ExternalId}; AssetCategory: {LifeCycleSetting.AssetCategoryName};";
        }
    }
}
