using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class AssetCategoryAddedDomainEvent : BaseEvent
    {

        public AssetCategoryAddedDomainEvent(AssetCategoryType addedCategory) : base(addedCategory.ExternalCustomerId)
        {
            AssetCategory = addedCategory;
        }

        public AssetCategoryType AssetCategory { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset category {Id} added.";
        }
    }
}