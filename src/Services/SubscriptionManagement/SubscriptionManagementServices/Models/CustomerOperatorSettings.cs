using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class CustomerOperatorSettings : Entity
    {
        public CustomerOperatorSettings(Operator @operator, ICollection<CustomerSubscriptionProduct> subscriptionProducts, ICollection<CustomerOperatorAccount> customerOperatorAccounts)
        {

            Operator = @operator;
            AvailableSubscriptionProducts = subscriptionProducts;
            CustomerOperatorAccounts = customerOperatorAccounts;
        }

        public CustomerOperatorSettings(Operator @operator)
        {
            Operator = @operator;
        }

        public CustomerOperatorSettings(){}

        public Operator Operator { get; protected set; }

        public ICollection<CustomerSubscriptionProduct> AvailableSubscriptionProducts { get; protected set; } = new List<CustomerSubscriptionProduct>();
        public ICollection<CustomerOperatorAccount> CustomerOperatorAccounts { get; protected set; } = new List<CustomerOperatorAccount>();
    }
}