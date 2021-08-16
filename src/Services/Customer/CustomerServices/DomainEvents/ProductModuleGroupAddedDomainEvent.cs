using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    class ProductModuleGroupAddedDomainEvent : BaseEvent
    {
        public ProductModuleGroupAddedDomainEvent(Guid customerId, ProductModuleGroup addedProductModuleGroup) : base(customerId)
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
