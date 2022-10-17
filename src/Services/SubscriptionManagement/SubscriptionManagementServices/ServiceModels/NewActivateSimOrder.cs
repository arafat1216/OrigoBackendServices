
using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.ServiceModels
{
    public record NewActivateSimOrder
    {
        [Phone]
        [MaxLength(15)]
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        [MaxLength(22)]
        public string SimCardNumber { get; set; }
        public string SimCardType { get; set; }
        public Guid CallerId { get; set; }
    }
}
