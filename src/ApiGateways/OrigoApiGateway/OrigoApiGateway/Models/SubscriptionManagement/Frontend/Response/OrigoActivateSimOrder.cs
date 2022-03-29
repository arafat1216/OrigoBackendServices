namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public class OrigoActivateSimOrder
    {
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public string OperatorName { get; set; }
        public string SimCardNumber { get; set; }
        public string SimCardType { get; set; }
    }
}
