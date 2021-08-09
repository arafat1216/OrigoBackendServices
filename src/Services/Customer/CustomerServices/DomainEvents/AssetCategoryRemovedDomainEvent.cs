using Common.Logging;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.DomainEvents
{
    class AssetCategoryRemovedDomainEvent : BaseEvent
    {
        public AssetCategoryRemovedDomainEvent(AssetCategoryType removedCategory) : base(removedCategory.AssetCategoryId)
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
