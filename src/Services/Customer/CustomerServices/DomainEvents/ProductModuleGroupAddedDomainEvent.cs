using Common.Logging;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.DomainEvents
{
    class ProductModuleGroupAddedDomainEvent : BaseEvent
    {
        public ProductModuleGroupAddedDomainEvent(ProductModuleGroup addedProductModuleGroup) : base(addedProductModuleGroup.ProductModuleGroupId)
        {
            ProductModuleGroup = addedProductModuleGroup;
        }

        public ProductModuleGroup ProductModuleGroup { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Product module {Id} removed.";
        }
    }
}
