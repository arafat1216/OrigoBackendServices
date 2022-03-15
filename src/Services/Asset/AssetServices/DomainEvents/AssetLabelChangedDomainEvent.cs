using Common.Logging;
using System;

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

        public override string EventMessage()
        {
            string state = (PreviousValue == true) ? "reactivated" : "deactivated"; // previous value was IsDeleted true, current false: meaning reactivate
            string assignedOrNot = (PreviousValue == true) ? "assigned to" : "unassigned from"; 
            return $"AssetLabel {AssetLabelExternalId} was {state}. Asset {AssetId} was {assignedOrNot} CustomerLabel {LabelId}.";
        }
    }
}
