using Common.Seedwork;
using System.ComponentModel.DataAnnotations;


namespace SubscriptionManagementServices.Models
{
    public class OperatorAccount : Entity
    {
        public OperatorAccount()
        {

        }
        public OperatorAccount(string accountNumber, string? accountName, Operator operatorTypeId, IList<SubscriptionOrder>? subscriptionOrders)
        {
            AccountNumber = accountNumber;
            AccountName = accountName;
            Operator = operatorTypeId;
            SubscriptionOrders = subscriptionOrders;
        }
        public Guid OrganizationId { get; set; }
        public string AccountNumber { get; set; }
        public string? AccountName { get; set;}
        [Required]
        public virtual Operator Operator { get; set; }
        public int OperatorId { get; set; }

        public virtual ICollection<SubscriptionOrder>? SubscriptionOrders { get; set; }
    }
}
