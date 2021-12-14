using AssetServices.Models;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.DomainEvents
{
    class NoteChangedDomainEvent<T> : BaseEvent where T:Asset
    {
        public T Asset { get; protected set; }
        public string PreviousNote { get; protected set; }

        public NoteChangedDomainEvent(T asset, string previousNote) : base(asset.ExternalId)
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
