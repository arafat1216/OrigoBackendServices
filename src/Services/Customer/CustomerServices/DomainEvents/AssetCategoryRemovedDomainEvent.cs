using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class AssetCategoryRemovedDomainEvent : BaseEvent
    {
        public AssetCategoryRemovedDomainEvent(AssetCategoryType removedCategory) : base(removedCategory.ExternalCustomerId)
        {
            AssetCategory = removedCategory;
        }

        public AssetCategoryType AssetCategory { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset category {Id} removed.";
        }
    }
}
