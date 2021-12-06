using AssetServices.Models;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.DomainEvents
{
    public class AssetLabelChangedDomainEvent : BaseEvent
    {
        public AssetLabelChangedDomainEvent(AssetLabel label, bool previousValue) : base(label.ExternalId)
        {
            Label = label;
            PreviousValue = previousValue;
        }

        public AssetLabel Label { get; protected set; }
        public bool PreviousValue { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            string state = (PreviousValue == true) ? "reactivated" : "deactivated";
            string assignedOrNot = (PreviousValue == true) ? "unassigned from" : "assigned to";
            return $"AssetLabel {Label.ExternalId} was {state}. Asset {Label.AssetId} was {assignedOrNot} CustomerLabel {Label.LabelId}.";
        }
    }
}
