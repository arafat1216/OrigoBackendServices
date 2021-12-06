using AssetServices.Models;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.DomainEvents
{
    public class AssetLabelCreatedDomainEvent : BaseEvent
    {
        public AssetLabelCreatedDomainEvent(AssetLabel label) : base(label.ExternalId)
        {
            Label = label;
        }

        public AssetLabel Label { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"AssetLabel {Label.ExternalId} created. Asset {Label.AssetId} was assigned to CustomerLabel {Label.LabelId}.";
        }
    }
}
