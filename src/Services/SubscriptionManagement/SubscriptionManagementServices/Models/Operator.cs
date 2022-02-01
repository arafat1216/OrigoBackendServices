using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class Operator : Entity
    {
        public Operator()
        {

        }
        public Operator(string operatorName, string country)
        {
            OperatorName = operatorName;
            Country = country;
        }

        public string OperatorName { get; set; }
        public string Country { get; set; }
        public virtual ICollection<SubscriptionProduct>? SubscriptionProducts { get; set; }
        public virtual ICollection<CustomerOperatorAccount>? CustomerOperatorAccounts { get; set; }

    }
}
