using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class CustomerOperatorSettings : Entity
    {
        public CustomerOperatorSettings(Operator @operator, IReadOnlyCollection<SubscriptionProduct>? subscriptionProducts, IReadOnlyCollection<CustomerOperatorAccount>? customerOperatorAccounts)
        {
            Operator = @operator;
            AvailableSubscriptionProducts = subscriptionProducts;
            CustomerOperatorAccounts = customerOperatorAccounts;
        }

        public CustomerOperatorSettings() { }

        public Operator Operator { get; protected set; }
        public IReadOnlyCollection<SubscriptionProduct>? AvailableSubscriptionProducts { get; protected set; }

        public IReadOnlyCollection<CustomerOperatorAccount>? CustomerOperatorAccounts { get; protected set; }
    }
}