using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public class OrigoSubscriptionOrderListItem
    {
        public Guid SubscriptionOrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OrderType { get; set; }
        public int OrderTypeId { get; set; }
        public string PhoneNumber { get; set; }
        public string NewSubscriptionOrderOwnerName { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public string CreatedBy { get; set; }
        public string OrderNumber { get; set; }
    }
}