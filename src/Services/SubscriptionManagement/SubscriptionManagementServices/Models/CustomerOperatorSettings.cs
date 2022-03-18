using Common.Seedwork;
using System.Text.Json.Serialization;

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

        public CustomerOperatorSettings(Operator @operator, ICollection<CustomerOperatorAccount>? customerOperatorAccounts, Guid callerId)
        {
            Operator = @operator;
            if (customerOperatorAccounts != null)
            {
                CustomerOperatorAccounts = customerOperatorAccounts;
            }
        }

        public CustomerOperatorSettings() { }

        [JsonIgnore]
        public Operator Operator { get; protected set; }
        [JsonIgnore]
        public CustomerStandardPrivateSubscriptionProduct StandardPrivateSubscriptionProduct { get; protected set; }
        [JsonIgnore]
        public ICollection<CustomerSubscriptionProduct> AvailableSubscriptionProducts { get; protected set; } = new List<CustomerSubscriptionProduct>();
        [JsonIgnore]
        public ICollection<CustomerOperatorAccount> CustomerOperatorAccounts { get; protected set; } = new List<CustomerOperatorAccount>();
    }
}