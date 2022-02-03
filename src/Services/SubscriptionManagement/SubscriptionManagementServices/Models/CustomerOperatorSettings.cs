using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class CustomerOperatorSettings : Entity
    {
        public Operator Operator { get; protected set; }
        public IReadOnlyCollection<SubscriptionProduct>? SubscriptionProducts { get; protected set; }
    }
}