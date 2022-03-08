
namespace SubscriptionManagementServices.ServiceModels
{
    public record NewActivateSimOrder
    {
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public string SimNumber { get; set; }
        public string SimType { get; set; }
        public Guid CallerId { get; set; }
    }
}
