using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class CustomerOperatorSettings : Entity
    {
        public CustomerOperatorSettings(Operator @operator, ICollection<SubscriptionProduct>? subscriptionProducts, IReadOnlyCollection<CustomerOperatorAccount>? customerOperatorAccounts)
        {
            Operator = @operator;
            AvailableSubscriptionProducts = subscriptionProducts;
            CustomerOperatorAccounts = customerOperatorAccounts;
        }

        public CustomerOperatorSettings() { }

        public int OperatorId { get; set; }
        public Guid CustomerId { get; set; }
        public Operator Operator { get; protected set; }
        public ICollection<SubscriptionProduct>? AvailableSubscriptionProducts { get; protected set; }
        public IReadOnlyCollection<CustomerOperatorAccount>? CustomerOperatorAccounts { get; protected set; }
    }
}