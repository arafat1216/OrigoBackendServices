

using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class DatapackagesAddedDomainEvent : BaseEvent
    {
        public DatapackagesAddedDomainEvent(List<DataPackage>? dataPackages, Guid customerId, Guid callerId)
        {
            DataPackages = dataPackages;
            CustomerId = customerId;
            CallerId = callerId;
        }

        public List<DataPackage>? DataPackages { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            //var datapackegesString = String.Join(", ", DataPackages.Select(a => a.DataPackageName));
            
            return $"Datapackges was added to customer id {CustomerId}";
        }
    }
}
