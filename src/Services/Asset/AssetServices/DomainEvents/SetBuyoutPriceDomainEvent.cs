using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents
{
    public class SetBuyoutPriceDomainEvent: BaseEvent
    {
        public SetBuyoutPriceDomainEvent(LifeCycleSetting lifeCycleSetting, Guid callerId) : base(lifeCycleSetting.ExternalId)
        {
            LifeCycleSetting = lifeCycleSetting;
            CallerId = callerId;
        }

        public LifeCycleSetting LifeCycleSetting { get; protected set; }
        public Guid CallerId { get; protected set; }

        public override string EventMessage()
        {
            return $"Min Buyout Price has been set for SettingId: {LifeCycleSetting.ExternalId}; AssetCategory: {LifeCycleSetting.AssetCategoryName} ; Amount: {LifeCycleSetting.MinBuyoutPrice}";
        }
    }
}
