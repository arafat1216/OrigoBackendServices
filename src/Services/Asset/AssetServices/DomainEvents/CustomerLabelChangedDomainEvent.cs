using AssetServices.Models;
using Common.Logging;

namespace AssetServices.DomainEvents
{
    public class CustomerLabelChangedDomainEvent : BaseEvent
    {
        public CustomerLabelChangedDomainEvent(CustomerLabel label, Label previousLabel) : base(label.ExternalId)
        {
            CustomerLabel = label;
            PreviousLabel = previousLabel;
        }

        public CustomerLabel CustomerLabel { get; protected set; }
        public Label PreviousLabel { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"CustomerLabel {Id}'s Label was changed: From {PreviousLabel.Text}/{PreviousLabel.Color} to {CustomerLabel.Label.Text}/{CustomerLabel.Label.Color}.";
        }
    }
}
