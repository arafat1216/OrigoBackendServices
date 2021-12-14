using AssetServices.Models;
using Common.Enums;
using Common.Logging;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class SetLifeCycleTypeDomainEvent<T> : BaseEvent where T:Asset
    {
        public T Asset { get; protected set; }
        public LifecycleType PreviousLifecycleType { get; protected set; }

        public SetLifeCycleTypeDomainEvent(T asset, LifecycleType previousLifecycleType) : base(asset.ExternalId)
        {
            Asset = asset;
            PreviousLifecycleType = previousLifecycleType;
        }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Lifecycle changed from {PreviousLifecycleType} to {Asset.LifecycleType}.";
        }
    }
}