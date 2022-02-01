using Common.Seedwork;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.Models
{
    public class Operator : Entity
    {
        public Operator()
        {

        }
        public Operator(string operatorName, string country, IList<SubscriptionProduct>? subscriptionProducts, IList<OperatorAccount>? operatorAccounts)
        {
            OperatorName = operatorName;
            Country = country;
            SubscriptionProducts = subscriptionProducts;
            OperatorAccounts = operatorAccounts;
        }

        [Required]
        public string OperatorName { get; set; }
        [Required]
        public string Country { get; set; }
        public virtual ICollection<SubscriptionProduct>? SubscriptionProducts { get; set; }
        public virtual ICollection<OperatorAccount>? OperatorAccounts { get; set; }

    }
}
