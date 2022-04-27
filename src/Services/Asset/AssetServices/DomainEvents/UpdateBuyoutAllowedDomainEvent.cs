using AssetServices.Models;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.DomainEvents
{
    public class UpdateBuyoutAllowedDomainEvent : BaseEvent
    {
        public UpdateBuyoutAllowedDomainEvent(LifeCycleSetting lifeCycleSetting, Guid callerId, bool previousStatus) : base(
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
            return $"LifeCycleSetting for setting id {LifeCycleSetting.ExternalId}; Changing 'Buyout Allowed' from {PreviousStatus}.";
        }
    }
}
