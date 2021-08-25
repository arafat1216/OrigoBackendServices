using AssetServices.Models;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.DomainEvents
{
    class NoteChangedDomainEvent : BaseEvent
    {
        public Asset Asset { get; protected set; }
        public string PreviousNote { get; protected set; }

        public NoteChangedDomainEvent(Asset asset, string previousNote) : base(asset.AssetId)
        {
            Asset = asset;
            PreviousNote = previousNote;
        }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset note changed from {PreviousNote} to {Asset.Note}.";
        }
    }
}
