using Common.Seedwork;
using System.ComponentModel.DataAnnotations;


namespace SubscriptionManagementServices.Models
{
    public class OperatorAccount : Entity
    {
        public OperatorAccount(string accountNumber, string? accountName, Operator operatorTypeId, IList<SubscriptionOrder>? subscriptions)
        {
            AccountNumber = accountNumber;
            AccountName = accountName;
            OperatorTypeId = operatorTypeId;
            Subscriptions = subscriptions;
        }

        public Guid OrganizationId { get; set; }
        public string AccountNumber { get; set; }
        public string? AccountName { get; set;}
        [Required]
        public Operator OperatorTypeId { get; set; }
        public IList<SubscriptionOrder>? Subscriptions { get; set; }
    }
}
