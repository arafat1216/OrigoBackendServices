
namespace SubscriptionManagementServices.ServiceModels
{
    public class ActivateSimOrderDTO
    {
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public string SimCardNumber { get; set; }
        public string SimCardType { get; set; }
    }
}
