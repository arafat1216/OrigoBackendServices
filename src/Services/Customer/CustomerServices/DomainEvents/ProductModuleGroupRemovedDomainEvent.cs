using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    class ProductModuleGroupRemovedDomainEvent : BaseEvent
    {
        public ProductModuleGroupRemovedDomainEvent(Guid customerId, ProductModuleGroup removedProductModuleGroup) : base(customerId)
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
