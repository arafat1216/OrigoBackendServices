using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class CustomerOperatorSettings : Entity
    {
        public CustomerOperatorSettings(int operatorId, ICollection<CustomerSubscriptionProduct>? subscriptionProducts, IReadOnlyCollection<CustomerOperatorAccount>? customerOperatorAccounts)
        {
            OperatorId = operatorId;
            AvailableSubscriptionProducts = subscriptionProducts;
            CustomerOperatorAccounts = customerOperatorAccounts;
        }

        public CustomerOperatorSettings() { }

        public int OperatorId { get; set; }
        public Operator Operator { get; protected set; }
        public ICollection<CustomerSubscriptionProduct>? AvailableSubscriptionProducts { get; protected set; }
        public IReadOnlyCollection<CustomerOperatorAccount>? CustomerOperatorAccounts { get; protected set; }
    }
}