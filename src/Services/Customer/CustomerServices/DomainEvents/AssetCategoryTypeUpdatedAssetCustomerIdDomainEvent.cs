using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class AssetCategoryTypeUpdatedAssetCustomerIdDomainEvent : BaseEvent
    {
        public AssetCategoryTypeUpdatedAssetCustomerIdDomainEvent(AssetCategoryType assetCategoryType, string oldCustomerId) : base(assetCategoryType.ExternalCustomerId)
        {
            AssetCategoryType = assetCategoryType;
            OldCustomerId = oldCustomerId;
        }
        public AssetCategoryType AssetCategoryType { get; protected set; }
        public string OldCustomerId { get; protected set; }
        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer id changed from {OldCustomerId} to {AssetCategoryType.ExternalCustomerId}.";
        }
    }
}
