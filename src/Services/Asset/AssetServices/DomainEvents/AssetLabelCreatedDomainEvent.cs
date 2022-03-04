using Common.Logging;
using System;

namespace AssetServices.DomainEvents
{
    public class AssetLabelCreatedDomainEvent : BaseEvent
    {
        public AssetLabelCreatedDomainEvent(Guid assetLabelExternalId, int assetId, int labelId) : base(assetLabelExternalId)
        {
            AssetLabelExternalId = assetLabelExternalId;
            AssetId = assetId;
            LabelId = labelId;
        }

        public Guid AssetLabelExternalId { get; protected set; }
        public int AssetId { get; protected set; }
        public int LabelId { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"AssetLabel {AssetLabelExternalId} created. Asset {AssetId} was assigned to CustomerLabel {LabelId}.";
        }
    }
}
