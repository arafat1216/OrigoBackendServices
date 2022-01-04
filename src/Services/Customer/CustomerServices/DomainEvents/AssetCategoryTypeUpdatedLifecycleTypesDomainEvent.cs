using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class AssetCategoryTypeUpdatedLifecycleTypesDomainEvent : BaseEvent
    {
        public AssetCategoryTypeUpdatedLifecycleTypesDomainEvent(AssetCategoryType assetCategoryType) : base(assetCategoryType.ExternalCustomerId)
        {
            AssetCategoryType = assetCategoryType;
        }
        public AssetCategoryType AssetCategoryType { get; protected set; }
        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer asset category lifecycle types changed.";
        }
    }
}
