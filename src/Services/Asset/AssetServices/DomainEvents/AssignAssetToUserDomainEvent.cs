using System;
using AssetServices.Models;
using Common.Logging;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.DomainEvents
{
    public class AssignAssetToUserDomainEvent<T> : BaseEvent where T:Asset
    {
        public AssignAssetToUserDomainEvent(T asset, Guid? previousUserId):base(asset.ExternalId)
        {
            Asset = asset;
            PreviousUserId = previousUserId;
        }

        public T Asset { get; protected set; }
        public Guid? PreviousUserId { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset assigned to user {Asset.AssetHolderId} from user {PreviousUserId}.";
        }
    }
}