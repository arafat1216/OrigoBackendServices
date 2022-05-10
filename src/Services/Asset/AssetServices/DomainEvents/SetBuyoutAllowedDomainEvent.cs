using AssetServices.Models;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.DomainEvents
{
    public class SetBuyoutAllowedDomainEvent : BaseEvent
    {
        public SetBuyoutAllowedDomainEvent(LifeCycleSetting lifeCycleSetting, Guid callerId, bool previousStatus) : base(
        lifeCycleSetting.ExternalId)
        {
            LifeCycleSetting = lifeCycleSetting;
            CallerId = callerId;
            PreviousStatus = previousStatus;
        }

        public LifeCycleSetting LifeCycleSetting { get; protected set; }
        public Guid CallerId { get; protected set; }
        public bool PreviousStatus { get; protected set; }

        public override string EventMessage()
        {
            return $"LifeCycleSetting for id {LifeCycleSetting.ExternalId}; Asset {LifeCycleSetting.AssetCategoryName}; Changing 'Buyout Allowed' from {PreviousStatus} to {LifeCycleSetting.BuyoutAllowed}.";
        }
    }
}
