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
        public AssetLabelChangedDomainEvent(Guid assetLabelExternalId, int assetId, int labelId, bool previousValue) : base(assetLabelExternalId)
        {
            AssetLabelExternalId = assetLabelExternalId;
            AssetId = assetId;
            LabelId = labelId;
            PreviousValue = previousValue;
        }

        public Guid AssetLabelExternalId { get;  protected set; }
        public int AssetId { get;  protected set; }
        public int LabelId { get; protected set; }
        public bool PreviousValue { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            string state = (PreviousValue == true) ? "reactivated" : "deactivated"; // previous value was IsDeleted true, current false: meaning reactivate
            string assignedOrNot = (PreviousValue == true) ? "assigned to" : "unassigned from"; 
            return $"AssetLabel {AssetLabelExternalId} was {state}. Asset {AssetId} was {assignedOrNot} CustomerLabel {LabelId}.";
        }
    }
}
