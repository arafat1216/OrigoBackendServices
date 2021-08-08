using System;
using AssetServices.Models;
using Common.Logging;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.DomainEvents
{
    public class AssignAssetToUserDomainEvent : BaseEvent
    {
        public AssignAssetToUserDomainEvent(Asset asset, Guid? previousUserId):base(asset.AssetId)
        {
            Asset = asset;
            PreviousUserId = previousUserId;
        }

        public Asset Asset { get; protected set; }
        public Guid? PreviousUserId { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset assigned to user {Asset.AssetHolderId} from user {PreviousUserId}.";
        }
    }
}