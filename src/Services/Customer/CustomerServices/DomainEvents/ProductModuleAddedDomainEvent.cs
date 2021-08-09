using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class ProductModuleAddedDomainEvent : BaseEvent
    {
        public ProductModuleAddedDomainEvent(ProductModule addedProductModule) : base(addedProductModule.ProductModuleId)
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
