using Common.Logging;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.DomainEvents
{
    class ProductModuleGroupRemovedDomainEvent : BaseEvent
    {
        public ProductModuleGroupRemovedDomainEvent(ProductModuleGroup removedProductModuleGroup) : base(removedProductModuleGroup.ProductModuleGroupId)
        {
            ProductModuleGroup = removedProductModuleGroup;
        }

        public ProductModuleGroup ProductModuleGroup { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Product module group {Id} removed.";
        }
    }
}
