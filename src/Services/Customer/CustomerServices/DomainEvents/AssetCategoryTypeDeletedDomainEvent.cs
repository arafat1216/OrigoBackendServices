using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class AssetCategoryTypeDeletedDomainEvent : BaseEvent
    {
        public AssetCategoryTypeDeletedDomainEvent(AssetCategoryType assetCategoryType) : base(assetCategoryType.ExternalCustomerId)
        {
            AssetCategoryType = assetCategoryType;
            DeletedBy = assetCategoryType.DeletedBy.ToString();
        }
        public AssetCategoryType AssetCategoryType { get; protected set; }
        public string DeletedBy { get; protected set; }
        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customers asset category type deleted by {DeletedBy}.";
        }
    }
}
