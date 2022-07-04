using AssetServices.Models;
using Common.Logging;

namespace AssetServices.DomainEvents
{
    public class CustomerLabelDeletedDomainEvent : BaseEvent
    {
        public CustomerLabelDeletedDomainEvent(CustomerLabel label, bool previousValue) : base(label.ExternalId)
        {
            CustomerLabel = label;
            PreviousDeleteState = previousValue;
        }

        public CustomerLabel CustomerLabel { get; protected set; }
        public bool PreviousDeleteState  { get; protected set; }

        public override string EventMessage()
        {
            return $"CustomerLabel {Id} was deleted.";
        }
    }
}
