using AssetServices.Models;
using Common.Logging;

namespace AssetServices.DomainEvents
{
    public class CustomerLabelCreatedDomainEvent : BaseEvent
    {
        public CustomerLabelCreatedDomainEvent(CustomerLabel label) : base(label.ExternalId)
        {
            CustomerLabel = label;
        }

        public CustomerLabel CustomerLabel { get; protected set; }

        public override string EventMessage()
        {
            return $"CustomerLabel {Id} created.";
        }
    }
}
