using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class CustomerOperatorSettings : Entity
    {
        public CustomerOperatorSettings(Operator @operator, IReadOnlyCollection<SubscriptionProduct>? subscriptionProducts, IReadOnlyCollection<CustomerOperatorAccount>? customerOperatorAccounts)
        {
            Operator = @operator;
            SubscriptionProducts = subscriptionProducts;
            CustomerOperatorAccounts = customerOperatorAccounts;
        }

        public CustomerOperatorSettings() { }

        public int OperatorId { get; set; }
        public Operator Operator { get; protected set; }
        public IReadOnlyCollection<SubscriptionProduct>? SubscriptionProducts { get; protected set; }
        public IReadOnlyCollection<CustomerOperatorAccount>? CustomerOperatorAccounts { get; protected set; }
    }
}