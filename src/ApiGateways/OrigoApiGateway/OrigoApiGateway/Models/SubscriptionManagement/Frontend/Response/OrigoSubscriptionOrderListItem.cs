using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public class OrigoSubscriptionOrderListItem
    {
        public DateTime CreatedDate { get; set; }
        public string OrderType { get; set; }
        public string PhoneNumber { get; set; }
        public string NewSubscriptionOrderOwnerName { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public string CreatedBy { get; set; }
    }
}