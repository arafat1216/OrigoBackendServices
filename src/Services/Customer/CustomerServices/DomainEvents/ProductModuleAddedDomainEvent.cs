using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    class ProductModuleAddedDomainEvent : BaseEvent
    {
        public ProductModuleAddedDomainEvent(Guid customerId, ProductModule addedProductModule) : base(customerId)
        {
            ProductModule = addedProductModule;
        }

        public ProductModule ProductModule { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Product module {Id} added.";
        }
    }
}
