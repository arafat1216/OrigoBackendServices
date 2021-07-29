using AssetServices.Models;
using Common.Logging;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class ModelChangedDomainEvent : BaseEvent
    {
        public Asset Asset { get; protected set; }
        public string PreviousModel { get; protected set; }

        public ModelChangedDomainEvent(Asset asset, string previousModel) : base(asset.AssetId)
        {
            Asset = asset;
            PreviousModel = previousModel;
        }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset changed from {PreviousModel} to {Asset.Model}.";
        }
    }
}