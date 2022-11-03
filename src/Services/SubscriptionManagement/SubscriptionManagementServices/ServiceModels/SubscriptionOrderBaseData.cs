using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.ServiceModels
{
    public class SubscriptionOrderBaseData
    {
        public Guid SubscriptionOrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int OrderTypeId { get; set; }
        public string OrderType { get; set; }
        public Guid CustomerId { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public Guid CreatedBy { get; set; }
        public string? OrderNumber { get; set; }
        public string PrivateFirstName { get; set; }
        public string PrivateLastName { get; set; }
        public string BusinessFirstName { get; set; }
        public string BusinessLastName { get; set; }
    }
}
