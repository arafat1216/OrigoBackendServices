using AssetServices.Models;
using Common.Logging;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class ModelChangedDomainEvent<T> : BaseEvent where T:Asset
    {
        public T Asset { get; protected set; }
        public string PreviousModel { get; protected set; }

        public ModelChangedDomainEvent(T asset, string previousModel) : base(asset.ExternalId)
        {
            Asset = asset;
            PreviousModel = previousModel;
        }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset changed from {PreviousModel} to {Asset.ProductName}.";
        }
    }
}