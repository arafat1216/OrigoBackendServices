using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class CustomerSettingsCreatedDomainEvent<T> : BaseEvent
    {
        public CustomerSettingsCreatedDomainEvent(CustomerSettings customerSettings, Guid callerId)
        {
            CustomerSettings = customerSettings;
            CallerId = callerId;
        }

        public CustomerSettings CustomerSettings { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer settings with customer id: {CustomerSettings.CustomerId} was created";
        }
    }
}
