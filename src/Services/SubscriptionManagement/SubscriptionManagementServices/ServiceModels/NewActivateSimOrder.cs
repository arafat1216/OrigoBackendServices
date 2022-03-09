
namespace SubscriptionManagementServices.ServiceModels
{
    public record NewActivateSimOrder
    {
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public string SimCardNumber { get; set; }
        public string SimCardType { get; set; }
        public Guid CallerId { get; set; }
    }
}
