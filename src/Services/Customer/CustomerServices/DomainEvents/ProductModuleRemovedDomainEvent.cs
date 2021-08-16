using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    class ProductModuleRemovedDomainEvent : BaseEvent
    {
        public ProductModuleRemovedDomainEvent(Guid customerId, ProductModule removedProductModule) : base(customerId)
        {
            ProductModule = removedProductModule;
        }

        public ProductModule ProductModule { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Product module {Id} removed.";
        }
    }
}