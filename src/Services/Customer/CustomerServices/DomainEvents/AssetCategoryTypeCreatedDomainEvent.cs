using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class AssetCategoryTypeCreatedDomainEvent : BaseEvent
    {
        public AssetCategoryTypeCreatedDomainEvent(AssetCategoryType assetCategoryType) : base(assetCategoryType.ExternalCustomerId)
        {
            AssetCategoryType = assetCategoryType;
        }
        public AssetCategoryType AssetCategoryType { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer {AssetCategoryType.ExternalCustomerId} created asset category with id {AssetCategoryType.AssetCategoryId}.";
        }
    }
}
