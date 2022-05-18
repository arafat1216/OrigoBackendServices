using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents
{
    public class SetBuyoutPriceDomainEvent: BaseEvent
    {
        public SetBuyoutPriceDomainEvent(LifeCycleSetting lifeCycleSetting, Guid customerId, Guid callerId) : base()
        {
            LifeCycleSetting = lifeCycleSetting;
            CallerId = callerId;
            CustomerId = customerId;
        }

        public LifeCycleSetting LifeCycleSetting { get; protected set; }
        public Guid CallerId { get; protected set; }
        public Guid CustomerId { get; protected set; }

        public override string EventMessage()
        {
            return $"Min Buyout Price has been set for Id: AssetCategory: {LifeCycleSetting.Id}; AssetCategory: {LifeCycleSetting.AssetCategoryName} ; Amount: {LifeCycleSetting.MinBuyoutPrice}";
        }
    }
}
