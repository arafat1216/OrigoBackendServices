using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class AssetCategoryTypeUpdatedAssetCategoryIdDomainEvent : BaseEvent
    {
        public AssetCategoryTypeUpdatedAssetCategoryIdDomainEvent(AssetCategoryType assetCategoryType, string oldCategoryId) : base(assetCategoryType.ExternalCustomerId)
        {
            AssetCategoryType = assetCategoryType;
            OldCategoryId = oldCategoryId;
        }
        public AssetCategoryType AssetCategoryType { get; protected set; }
        public string OldCategoryId { get; protected set; }
        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer category id changed from {OldCategoryId} to {AssetCategoryType.AssetCategoryId}.";
        }
    }
}
