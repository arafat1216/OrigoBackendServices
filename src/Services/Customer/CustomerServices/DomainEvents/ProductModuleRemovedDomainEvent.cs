using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class ProductModuleRemovedDomainEvent : BaseEvent
    {
        public ProductModuleRemovedDomainEvent(ProductModule removedProductModule) : base(removedProductModule.ProductModuleId)
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