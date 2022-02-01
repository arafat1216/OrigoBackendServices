using Common.Seedwork;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.Models
{
    public class Operator : Entity
    {
        public Operator( string operatorName, string country)
        {
            OperatorName = operatorName;
            Country = country;
        }
        [Required]
        public string OperatorName { get; set; }
        [Required]
        public string Country { get; set; }
        public IList<SubscriptionProduct>? SubscriptionTypes { get; set; }

    }
}
